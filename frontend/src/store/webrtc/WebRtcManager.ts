import { EventEmitter } from 'events';
import { RtpCapabilities } from 'mediasoup-client/lib/types';
import appHubConn from '../signal/app-hub-connection';
import { WebRtcConnection } from './WebRtcConnection';

type WebRtcOptions = {
   sendMedia?: boolean;
   receiveMedia?: boolean;
};

export class WebRtcManager extends EventEmitter {
   private _current: WebRtcConnection | undefined;

   get current(): WebRtcConnection | undefined {
      return this._current;
   }

   public async initialize({ sendMedia, receiveMedia }: WebRtcOptions): Promise<void> {
      const signalr = appHubConn.current;
      if (!signalr) {
         throw new Error('SignalR connection is not available.');
      }

      const connection = new WebRtcConnection(signalr);
      const device = connection.device;

      const routerRtpCapabilities = await signalr.invoke<RtpCapabilities>('RequestRouterCapabilities');
      if (!routerRtpCapabilities) throw new Error('Router capabilities could not be retrived from server.');

      await device.load({ routerRtpCapabilities });

      const result = await signalr.invoke('InitializeConnection', {
         sctpCapabilities: device.sctpCapabilities,
         rtpCapabilities: device.rtpCapabilities,
      });

      if (result) return;

      const canProduceAudio = device.canProduce('audio');
      const canProduceVideo = device.canProduce('video');

      if (sendMedia && (canProduceAudio || canProduceVideo)) {
         await connection.createSendTransport();
      }

      if (receiveMedia) {
         await connection.createReceiveTransport();
      }

      this._current = connection;
      this.emit('update');
   }
}
