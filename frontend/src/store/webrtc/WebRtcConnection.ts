import { HubConnection } from '@microsoft/signalr';
import { EventEmitter } from 'events';
import { Device } from 'mediasoup-client';
import { Consumer, MediaKind, RtpParameters, Transport, TransportOptions } from 'mediasoup-client/lib/types';
import { ChangeStreamDto } from './types';
import * as coreHub from 'src/core-hub';
import { HubSubscription, subscribeEvent, unsubscribeAll } from 'src/utils/signalr-utils';
import { SuccessOrError } from 'src/communication-types';

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
   /** all current consumers */
   private consumers = new Map<string, Consumer>();

   /** the latest received score for all consumers */
   private consumerScores = new Map<string, number>();

   /** SignalR methods that were subscribed. Must be memorized for unsubscription in close() */
   private signalrSubscription = new Array<HubSubscription>();

   constructor(private connection: HubConnection) {
      this.device = new Device();

      this.signalrSubscription.push(
         subscribeEvent(connection, 'newConsumer', this.onNewConsumer.bind(this)),
         subscribeEvent(connection, 'consumerClosed', this.onConsumerClosed.bind(this)),
         subscribeEvent(connection, 'consumerScore', this.onConsumerScore.bind(this)),

         subscribeEvent(connection, 'consumerPaused', this.onConsumerPaused.bind(this)),
         subscribeEvent(connection, 'consumerResumed', this.onConsumerResumed.bind(this)),
      );
   }

   public eventEmitter = new EventEmitter();

   public device: Device;
   public sendTransport: Transport | null = null;
   public receiveTransport: Transport | null = null;

   public canProduceVideo() {
      return this.device.canProduce('video');
   }

   public canProduceAudio() {
      return this.device.canProduce('audio');
   }

   public getConsumers(): IterableIterator<Consumer> {
      return this.consumers.values();
   }

   public close(): void {
      this.sendTransport?.close();
      this.receiveTransport?.close();

      for (const consumer of this.consumers.values()) {
         consumer.close();
      }

      this.consumers.clear();
      this.eventEmitter.emit('onConsumersChanged');

      unsubscribeAll(this.connection, this.signalrSubscription);
   }

   private async onNewConsumer({ id, producerId, kind, rtpParameters, appData, participantId }: OnNewConsumerPayload) {
      if (!this.receiveTransport) {
         console.warn('received new consumer, but receive transport is not initialized');
         return;
      }

      try {
         const consumer = await this.receiveTransport.consume({
            id,
            rtpParameters,
            kind,
            producerId,
            appData: { ...appData, participantId },
         });

         this.consumers.set(consumer.id, consumer);

         consumer.on('transportclose', () => this.consumers.delete(consumer.id));

         this.eventEmitter.emit('onConsumersChanged');
      } catch (error) {
         console.log(error);

         // TODO: Handle
      }
   }

   private onConsumerClosed({ consumerId }: ConsumerInfoPayload) {
      const consumer = this.consumers.get(consumerId);
      if (consumer) {
         consumer.close();
         this.consumers.delete(consumerId);
         this.consumerScores.delete(consumerId);
         this.eventEmitter.emit('onConsumerUpdated', { consumerId });
      }
   }

   private onConsumerPaused({ consumerId }: ConsumerInfoPayload) {
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

   public async createSendTransport(): Promise<Transport> {
      const request = {
         sctpCapabilities: this.device.sctpCapabilities,
         producing: true,
         consuming: false,
      };
      const transportOptions = await this.connection.invoke<SuccessOrError<TransportOptions>>(
         'CreateWebRtcTransport',
         request,
      );
      if (!transportOptions.success) {
         console.error('Error creating send transport: ', transportOptions.error);
         throw new Error('Error creating send transport.');
      }

      const transport = this.device.createSendTransport({
         ...transportOptions.response,
         iceServers: [],
         proprietaryConstraints: PC_PROPRIETARY_CONSTRAINTS,
      });

      transport.on('connect', async ({ dtlsParameters }, callback, errback) =>
         this.connection
            .invoke<SuccessOrError<any>>('ConnectWebRtcTransport', { transportId: transport.id, dtlsParameters })
            .then((response) => {
               console.log('connect response', response);
               if (response.success) callback();
               else errback();
            })
            .catch(errback),
      );

      transport.on('produce', async ({ kind, rtpParameters, appData }, callback, errback) => {
         try {
            const result = await this.connection.invoke<SuccessOrError<any>>('ProduceWebRtcTransport', {
               transportId: transport.id,
               kind,
               rtpParameters,
               appData,
            });

            if (result.success) {
               callback({ id: result.response.id });
            } else {
               errback(result.error);
            }
         } catch (error) {
            errback(error);
         }
      });

      this.sendTransport = transport;
      return transport;
   }

   public async createReceiveTransport(): Promise<Transport> {
      const transportOptions = await this.connection.invoke<SuccessOrError<TransportOptions>>('CreateWebRtcTransport', {
         producing: false,
         consuming: true,
      });

      if (!transportOptions.success) {
         console.error('Error creating receive transport: ', transportOptions.error);
         throw new Error('Error creating receive transport.');
      }

      const transport = this.device.createRecvTransport(transportOptions.response);

      transport.on('connect', ({ dtlsParameters }, callback, errback) => {
         this.connection
            .invoke<SuccessOrError<any>>('ConnectWebRtcTransport', { transportId: transport.id, dtlsParameters })
            .then((response) => {
               if (response.success) callback(response.response);
               else errback(response.error);
            })
            .catch(errback);
      });

      this.receiveTransport = transport;
      return transport;
   }

   public async changeStream(dto: ChangeStreamDto): Promise<void> {
      const result = await this.connection.invoke<SuccessOrError<void>>(coreHub.changeStream, dto);
      if (!result.success) {
         throw new Error(result.error.message);
      }
   }
}
