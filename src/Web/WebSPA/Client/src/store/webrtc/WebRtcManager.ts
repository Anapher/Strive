import debug from 'debug';
import { SuccessOrError } from 'src/communication-types';
import { SfuConnectionInfo } from 'src/core-hub.types';
import { sleep } from 'src/utils/promise-utils';
import { TypedEmitter } from 'tiny-typed-emitter';
import appHubConn from '../signal/app-hub-connection';
import SfuClient from './sfu-client';
import { WebRtcConnection } from './WebRtcConnection';

const RECONNECT_DELAY = 2000;
const log = debug('webrtc:manager');

export type WebRtcOptions = {
   sendMedia?: boolean;
   receiveMedia?: boolean;
};

export type WebRtcStatus = 'uninitialized' | 'connecting' | 'connected';

interface WebRtcManagerEvents {
   update: () => void;
   statuschanged: () => void;
   errorOccurred: () => void;
}

/**
 * Manager for {@see WebRtcConnection}. This class will initialize the connection and reconnect if the connection is lost
 */
export class WebRtcManager extends TypedEmitter<WebRtcManagerEvents> {
   private _current: WebRtcConnection | undefined;

   constructor(private options: WebRtcOptions) {
      super();
   }

   public status: WebRtcStatus = 'uninitialized';
   public latestError: any | undefined;

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

      appHubConn.on('update', () => this.tryConnect());
      this.tryConnect();
   }

   private async tryConnect(): Promise<void> {
      this.status = 'connecting';
      this.onStatusChanged();

      while (appHubConn.current) {
         try {
            await this.initialize();
            this.status = 'connected';
            this.latestError = undefined;
            this.onStatusChanged();
            break;
         } catch (error) {
            this.setLatestError(error);
            log('Error initializing WebRtc connection: %O', error);
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

      const connectionInfo = await signalr.invoke<SuccessOrError<SfuConnectionInfo>>('FetchSfuConnectionInfo');
      if (!connectionInfo?.success) throw new Error('Unable to fetch SFU connection info');
      log('Received connection info %O', connectionInfo);

      const client = new SfuClient(connectionInfo.response);

      const connection = new WebRtcConnection(signalr, client);
      const device = connection.device;

      const rtpResult = await client.getRouterCapabilities();
      if (!rtpResult?.success) throw new Error('Router capabilities could not be retrived from server.');
      log('Received router capabilities %O', rtpResult);

      await device.load({ routerRtpCapabilities: rtpResult.response });

      const result = await client.initializeConnection({
         sctpCapabilities: device.sctpCapabilities,
         rtpCapabilities: device.rtpCapabilities,
      });

      if (!result.success) {
         throw new Error('Initialize connection failed, empty result.');
      }

      log('Initialized device');

      const canProduceAudio = device.canProduce('audio');
      const canProduceVideo = device.canProduce('video');

      const { sendMedia, receiveMedia } = this.options;

      if (sendMedia && (canProduceAudio || canProduceVideo)) {
         await connection.createSendTransport();
         log('Send transport created');
      }

      if (receiveMedia) {
         const transport = await connection.createReceiveTransport();
         transport.on('connectionstatechange', (state: RTCPeerConnectionState) => {
            if (state === 'disconnected') {
               log('WebRTC transport disconnected, attempt to reconnect...');
               this.onDisconnected();
            }
         });

         log('Receive transport created');
      }

      this._current = connection;
      this.onCurrentChanged();
   }

   private onCurrentChanged() {
      this.emit('update');
   }

   private onStatusChanged() {
      this.emit('statuschanged');
      log('Set status to %s', this.status);
   }

   private setLatestError(error: any) {
      this.latestError = error;
      this.emit('errorOccurred');
   }
}
