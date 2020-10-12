import { HubConnection } from '@microsoft/signalr';
import { PayloadAction } from '@reduxjs/toolkit';
import { AnyAction, Dispatch, Middleware, MiddlewareAPI } from 'redux';
import { initializationFailed, initialize, initialized } from './actions';
import { SoupManager } from './SoupManager';

export type RtcListener = {
   middleware: Middleware;
   getSoupManager: () => SoupManager | undefined;
};

export default function createRtcManager(getConnection: () => HubConnection | undefined): RtcListener {
   let dispatch: Dispatch<AnyAction>;
   let soupManager: SoupManager | undefined;

   const middleware: Middleware = (store: MiddlewareAPI) => {
      dispatch = store.dispatch;

      return (next) => async (action: PayloadAction<any>) => {
         switch (action.type) {
            case initialize.type: {
               const connection = getConnection();
               if (!connection) {
                  dispatch(initializationFailed);
                  return;
               }

               soupManager = new SoupManager(dispatch, connection);
               await soupManager.initializeDevice();

               const { device } = soupManager;

               const canProduceAudio = device.canProduce('audio');
               const canProduceVideo = device.canProduce('video');

               await connection.invoke('InitializeConnection', {
                  sctpCapabilities: device.sctpCapabilities,
                  rtpCapabilities: device.rtpCapabilities,
               });

               dispatch(initialized({ canProduceAudio, canProduceVideo }));

               if (canProduceAudio || canProduceVideo) {
                  await soupManager.createSendTransport();
               }

               await soupManager.createReceiveTransport();
               break;
            }
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
