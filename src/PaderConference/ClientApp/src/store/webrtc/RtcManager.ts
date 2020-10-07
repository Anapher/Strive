import { onConferenceJoined, onEventOccurred, send, subscribeEvent } from 'src/store/conference-signal/actions';
import { AnyAction, Dispatch, Middleware, MiddlewareAPI } from 'redux';
import { PayloadAction } from '@reduxjs/toolkit';

class RtcManager {
   constructor(private dispatch: Dispatch<AnyAction>) {
      this.connection = null;
   }

   private connection: RTCPeerConnection | null;

   public getConnection(): RTCPeerConnection | null {
      return this.connection;
   }

   public createConnection(): void {
      if (this.connection) {
         this.connection.close();
      }

      const conn = new RTCPeerConnection();
      conn.onicecandidate = ({ candidate }) => {
         if (candidate) this.dispatch(send('RtcSendIceCandidate', candidate));
      };
      conn.onnegotiationneeded = async () => {
         try {
            await conn.setLocalDescription(await conn.createOffer());

            // send the offer to the other peer
            this.dispatch(send('RtcSetDescription', conn.localDescription));
         } catch (err) {
            console.error(err);
         }
      };

      this.connection = conn;
   }
}

export type RtcListener = {
   middleware: Middleware;
   getRtcManager: () => RtcManager;
};

export default function createRtcManager(): RtcListener {
   let dispatch: Dispatch<AnyAction>;
   let rtcManager: RtcManager | undefined;

   const middleware: Middleware = (store: MiddlewareAPI) => {
      dispatch = store.dispatch;

      return (next) => async (action: PayloadAction<any>) => {
         switch (action.type) {
            case onEventOccurred('OnIceCandidate').type:
               console.log('OnIceCandidate');

               rtcManager?.getConnection()?.addIceCandidate(action.payload as any);
               break;
            case onEventOccurred('OnSdp').type:
               console.log('OnSdp');

               // eslint-disable-next-line no-case-declarations
               const sessionDesc = action.payload as RTCSessionDescriptionInit;
               // eslint-disable-next-line no-case-declarations
               const conn = rtcManager?.getConnection();
               if (!conn) return;

               conn.setRemoteDescription(sessionDesc);
               if (sessionDesc.type === 'offer') {
                  await conn.setLocalDescription(await conn.createAnswer());
                  dispatch(send('RtcSetDescription', conn.localDescription));
               }
               break;
            case onConferenceJoined.type:
               dispatch(subscribeEvent('OnSdp'));
               dispatch(subscribeEvent('OnIceCandidate'));
               break;
            default:
               break;
         }

         return next(action);
      };
   };

   const getRtcManager = () => {
      if (!dispatch) {
         throw new Error('The rtc middleware is not installed');
      }

      if (rtcManager) return rtcManager;
      return (rtcManager = new RtcManager(dispatch));
   };

   return { middleware, getRtcManager };
}
