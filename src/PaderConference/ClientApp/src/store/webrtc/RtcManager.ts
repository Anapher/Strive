import { onConferenceJoined, onEventOccurred, send, subscribeEvent } from 'src/store/conference-signal/actions';
import { AnyAction, Dispatch, Middleware, MiddlewareAPI } from 'redux';
import { PayloadAction } from '@reduxjs/toolkit';
import { Device } from 'mediasoup-client';
import { initialize, initialized } from './actions';
import { Producer, Transport, TransportOptions } from 'mediasoup-client/lib/types';
import { JsonPatchError } from 'fast-json-patch';

export class SoupManager {
   constructor(private dispatch: Dispatch<AnyAction>) {
      this.device = new Device();
   }

   private micProducer?: Producer;

   public device: Device;
   public sendTransport: Transport | null = null;

   public createSendTransport(options: TransportOptions): void {
      this.sendTransport = this.device.createSendTransport(options);

      this.sendTransport.on('connect', ({ dtlsParameters }, callback, errback) => {
         // wait
      });
   }
}

export type RtcListener = {
   middleware: Middleware;
   getSoupManager: () => SoupManager | undefined;
};

export default function createRtcManager(): RtcListener {
   let dispatch: Dispatch<AnyAction>;
   let soupManager: SoupManager | undefined;

   const middleware: Middleware = (store: MiddlewareAPI) => {
      dispatch = store.dispatch;

      return (next) => async (action: PayloadAction<any>) => {
         switch (action.type) {
            case onEventOccurred('OnTransportCreated').type:
               if (soupManager) {
                  soupManager.createSendTransport(action.payload);

                  const producer = soupManager.sendTransport?.produce({});
               }
               break;
            case onEventOccurred('OnRouterCapabilities').type:
               if (soupManager) {
                  const { device } = soupManager;
                  await device.load({ routerRtpCapabilities: action.payload });

                  const canProduceAudio = device.canProduce('audio');
                  const canProduceVideo = device.canProduce('video');

                  dispatch(initialized({ canProduceAudio, canProduceVideo }));

                  if (canProduceAudio || canProduceVideo) {
                     dispatch(
                        send('CreateTransport', {
                           sctpCapabilities: device.sctpCapabilities,
                        }),
                     );
                  }
               }
               break;
            case initialize.type:
               dispatch(subscribeEvent('OnRouterCapabilities'));
               dispatch(subscribeEvent('OnTransportCreated'));

               soupManager = new SoupManager(dispatch);
               dispatch(send('RequestRouterCapabilities'));
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

      return soupManager;
   };

   return { middleware, getSoupManager: getRtcManager };
}
