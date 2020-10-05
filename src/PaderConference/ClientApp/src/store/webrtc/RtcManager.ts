import { onEventOccurred, send, subscribeEvent } from 'src/store/conference-signal/actions';
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
   let eventsInitialized = false;

   const middleware: Middleware = (store: MiddlewareAPI) => {
      dispatch = store.dispatch;

      return (next) => (action: PayloadAction) => {
         switch (action.type) {
            case onEventOccurred('OnIceCandidate').type:
               console.log('OnIceCandidate');

               rtcManager?.getConnection()?.addIceCandidate(action.payload as any);
               break;
            case onEventOccurred('OnSdp').type:
               console.log('OnSdp');

               rtcManager?.getConnection()?.setRemoteDescription(action.payload as any);
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

      if (!eventsInitialized) {
         eventsInitialized = true;
         dispatch(subscribeEvent('OnSdp'));
         dispatch(subscribeEvent('OnIceCandidate'));
      }

      return (rtcManager = new RtcManager(dispatch));
   };

   return { middleware, getRtcManager };
}
