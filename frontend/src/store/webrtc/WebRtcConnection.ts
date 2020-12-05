import { HubConnection } from '@microsoft/signalr';
import { EventEmitter } from 'events';
import { Device } from 'mediasoup-client';
import { Consumer, MediaKind, RtpParameters, Transport, TransportOptions } from 'mediasoup-client/lib/types';
import { ChangeStreamDto } from './types';
import * as coreHub from 'src/core-hub';

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

type ConsumerScorePayload = ConsumerInfoPayload & {
   consumerId: string;
   score: number;
};

export class WebRtcConnection {
   private consumers = new Map<string, Consumer>();
   private consumerScores = new Map<string, number>();

   constructor(private connection: HubConnection) {
      this.device = new Device();

      connection.on('newConsumer', this.onNewConsumer.bind(this));
      connection.on('consumerClosed', this.onConsumerClosed.bind(this));
      connection.on('consumerScore', this.onConsumerScore.bind(this));

      connection.on('consumerPaused', this.onConsumerPaused.bind(this));
      connection.on('consumerResumed', this.onConsumerResumed.bind(this));
   }

   public eventEmitter = new EventEmitter();

   public canProduceVideo() {
      return this.device.canProduce('video');
   }

   public canProduceAudio() {
      return this.device.canProduce('audio');
   }

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
         console.log(error);

         // TODO: Handle
      }
   }

   private onConsumerClosed({ consumerId }: ConsumerInfoPayload) {
      console.log('on consumer closed');

      const consumer = this.consumers.get(consumerId);
      if (consumer) {
         consumer.close();
         this.consumers.delete(consumerId);
         this.consumerScores.delete(consumerId);
         this.eventEmitter.emit('onConsumerUpdated', { consumerId });
      }
   }

   private onConsumerPaused({ consumerId }: ConsumerInfoPayload) {
      console.log('paused');

      const consumer = this.consumers.get(consumerId);
      if (consumer) {
         consumer.pause();
         this.consumerScores.delete(consumerId);
         this.eventEmitter.emit('onConsumerUpdated', { consumerId });
      }
   }

   private onConsumerResumed({ consumerId }: ConsumerInfoPayload) {
      const consumer = this.consumers.get(consumerId);
      if (consumer) {
         consumer.resume();
         this.eventEmitter.emit('onConsumerUpdated', { consumerId });
      }
   }

   private onConsumerScore({ consumerId, score }: ConsumerScorePayload) {
      this.consumerScores.set(consumerId, score);
      this.eventEmitter.emit('onConsumerScoreUpdated', { consumerId });
   }

   public device: Device;
   public sendTransport: Transport | null = null;
   public receiveTransport: Transport | null = null;

   public async createSendTransport(): Promise<Transport> {
      const request = {
         sctpCapabilities: this.device.sctpCapabilities,
         producing: true,
         consuming: false,
      };
      const transportOptions = await this.connection.invoke<TransportOptions>('CreateWebRtcTransport', request);

      const transport = this.device.createSendTransport({
         ...transportOptions,
         iceServers: [],
         proprietaryConstraints: PC_PROPRIETARY_CONSTRAINTS,
      });

      transport.on('connect', async ({ dtlsParameters }, callback, errback) =>
         this.connection
            .invoke('ConnectWebRtcTransport', { transportId: transport.id, dtlsParameters })
            .then(callback)
            .catch(errback),
      );

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
      return transport;
   }

   public async changeStream(dto: ChangeStreamDto): Promise<void> {
      await this.connection.invoke(coreHub.changeStream, dto);
   }
}
