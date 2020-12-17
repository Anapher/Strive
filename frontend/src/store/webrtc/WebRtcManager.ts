import { EventEmitter } from 'events';
import { RtpCapabilities } from 'mediasoup-client/lib/types';
import { SuccessOrError } from 'src/communication-types';
import { sleep } from 'src/utils/promise-utils';
import appHubConn from '../signal/app-hub-connection';
import { WebRtcConnection } from './WebRtcConnection';

const RECONNECT_DELAY = 2000;

export type WebRtcOptions = {
   sendMedia?: boolean;
   receiveMedia?: boolean;
};

export type WebRtcStatus = 'uninitialized' | 'connecting' | 'connected';

/**
 * Manager for {@see WebRtcConnection}. This class will initialize the connection and reconnect if the connection is lost
 */
export class WebRtcManager extends EventEmitter {
   private _current: WebRtcConnection | undefined;

   constructor(private options: WebRtcOptions) {
      super();
   }

   public status: WebRtcStatus = 'uninitialized';

   get current(): WebRtcConnection | undefined {
      return this._current;
   }

   /**
    * Begin the connection process. This method will attempt to get the required WebRTC information from server and create a {@see WebRtcConnection}.
    * The reconnection process is automatically handled by this class.
    */
   public beginConnecting() {
      // ensure that only one connection loop is activated
      if (this.status !== 'uninitialized') return;

      appHubConn.eventEmitter.on('update', () => this.tryConnect());
      this.tryConnect();
   }

   private async tryConnect(): Promise<void> {
      this.status = 'connecting';
      this.onStatusChanged();

      while (appHubConn.current) {
         try {
            await this.initialize();
            this.status = 'connected';
            this.onStatusChanged();
            break;
         } catch (error) {
            console.error('Error initializing WebRtc connection', error);
         }

         await sleep(RECONNECT_DELAY);
      }
   }

   private async onDisconnected() {
      this._current?.close();
      this._current = undefined;
      this.onCurrentChanged();

      this.tryConnect();
   }

   private async initialize(): Promise<void> {
      const signalr = appHubConn.current;
      if (!signalr) {
         throw new Error('SignalR connection is not available.');
      }

      const connection = new WebRtcConnection(signalr);
      const device = connection.device;

      const rtpResult = await signalr.invoke<SuccessOrError<RtpCapabilities>>('RequestRouterCapabilities');
      if (!rtpResult?.success) throw new Error('Router capabilities could not be retrived from server.');

      console.log(rtpResult);

      await device.load({ routerRtpCapabilities: rtpResult.response });

      const result = await signalr.invoke<SuccessOrError<void>>('InitializeConnection', {
         sctpCapabilities: device.sctpCapabilities,
         rtpCapabilities: device.rtpCapabilities,
      });

      if (!result.success) {
         throw new Error('Initialize connection failed, empty result.');
      }

      const canProduceAudio = device.canProduce('audio');
      const canProduceVideo = device.canProduce('video');

      const { sendMedia, receiveMedia } = this.options;

      if (sendMedia && (canProduceAudio || canProduceVideo)) {
         await connection.createSendTransport();
      }

      if (receiveMedia) {
         const transport = await connection.createReceiveTransport();
         transport.on('connectionstatechange', (state: RTCPeerConnectionState) => {
            if (state === 'disconnected') {
               console.log('WebRTC transport disconnected, attempt to reconnect...');
               this.onDisconnected();
            }
         });
      }

      this._current = connection;
      this.onCurrentChanged();
   }

   private onCurrentChanged() {
      this.emit('update');
   }

   private onStatusChanged() {
      this.emit('statuschanged');
   }
}
