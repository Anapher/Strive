import { HubConnection } from '@microsoft/signalr';
import { EventEmitter } from 'events';
import { Device } from 'mediasoup-client';
import {
   Consumer,
   MediaKind,
   RtpCapabilities,
   RtpParameters,
   Transport,
   TransportOptions,
} from 'mediasoup-client/lib/types';
import { AnyAction, Dispatch } from 'redux';

const PC_PROPRIETARY_CONSTRAINTS = {
   optional: [{ googDscp: true }],
};

type OnNewConsumerPayload = {
   participantId: string;
   producerId: string;
   id: string;
   kind: MediaKind;
   rtpParameters: RtpParameters;
   type: string;
   appData: any;
   producerPaused: boolean;
};

type ConsumerInfoPayload = {
   consumerId: string;
};

export class SoupManager {
   private consumers = new Map<string, Consumer>();

   constructor(private dispatch: Dispatch<AnyAction>, private connection: HubConnection) {
      this.device = new Device();

      connection.on('newConsumer', this.onNewConsumer.bind(this));
      connection.on('consumerClosed', this.onConsumerClosed.bind(this));
   }

   public eventEmitter = new EventEmitter();

   public getConsumers(): IterableIterator<Consumer> {
      return this.consumers.values();
   }

   private async onNewConsumer({ id, producerId, kind, rtpParameters, appData, participantId }: OnNewConsumerPayload) {
      console.log('on new consumer');

      if (!this.receiveTransport) return;

      try {
         const consumer = await this.receiveTransport.consume({
            id,
            rtpParameters,
            kind,
            producerId,
            appData: { ...appData, participantId },
         });

         console.log('consumer received');

         this.consumers.set(consumer.id, consumer);

         consumer.on('transportclose', () => this.consumers.delete(consumer.id));

         this.eventEmitter.emit('onConsumersChanged');
      } catch (error) {
         // TODO: Handle
      }
   }

   private async onConsumerClosed({ consumerId }: ConsumerInfoPayload) {
      const consumer = this.consumers.get(consumerId);
      if (consumer) {
         consumer.close();
         this.consumers.delete(consumerId);
      }
   }

   public device: Device;
   public sendTransport: Transport | null = null;
   public receiveTransport: Transport | null = null;

   public async initializeDevice(): Promise<boolean> {
      const routerRtpCapabilities = await this.connection.invoke<RtpCapabilities>('RequestRouterCapabilities');
      if (!routerRtpCapabilities) return false;

      await this.device.load({ routerRtpCapabilities });
      return true;
   }

   public async createSendTransport(): Promise<Transport> {
      const request = {
         sctpCapabilities: this.device.sctpCapabilities,
         producing: true,
         consuming: false,
      };
      const transportOptions = await this.connection.invoke<TransportOptions>('CreateWebRtcTransport', request);
      console.log(transportOptions);

      const transport = this.device.createSendTransport({
         ...transportOptions,
         iceServers: [],
         proprietaryConstraints: PC_PROPRIETARY_CONSTRAINTS,
      });

      transport.on('connect', ({ dtlsParameters }, callback, errback) => {
         this.connection
            .invoke('ConnectWebRtcTransport', { transportId: transport.id, dtlsParameters })
            .then(callback)
            .catch(errback);
      });

      transport.on('produce', async ({ kind, rtpParameters, appData }, callback, errback) => {
         try {
            const { id } = await this.connection.invoke('ProduceWebRtcTransport', {
               transportId: transport.id,
               kind,
               rtpParameters,
               appData,
            });

            callback({ id });
         } catch (error) {
            errback(error);
         }
      });

      transport.on('connectionstatechange', (state) => console.log('connection state ', state));

      this.sendTransport = transport;
      return transport;
   }

   public async createReceiveTransport(): Promise<Transport> {
      const transportOptions = await this.connection.invoke<TransportOptions>('CreateWebRtcTransport', {
         producing: false,
         consuming: true,
      });

      const transport = this.device.createRecvTransport(transportOptions);

      transport.on('connect', ({ dtlsParameters }, callback, errback) => {
         this.connection
            .invoke('ConnectWebRtcTransport', { transportId: transport.id, dtlsParameters })
            .then(callback)
            .catch(errback);
      });

      this.receiveTransport = transport;
      console.log('receive transport created');
      return transport;
   }
}
