import { Consumer, Producer, Router } from 'mediasoup/lib/types';
import Connection from '../connection';
import Logger from '../logger';
import { ISignalWrapper } from '../signal-wrapper';

const logger = new Logger('MediasoupMixer');

type ProducerInfo = {
   producer: Producer;
   participantId: string;
};

/**
 * A mixer redirects producers to receivers. All producers added are immediately consumed by the receivers.
 */
export class MediasoupMixer {
   private producers = new Map<string, ProducerInfo>();
   private receivers = new Map<string, Connection>();

   constructor(private router: Router, private signal: ISignalWrapper) {}

   /**
    * Add a new producer and consume it by all receivers of this mixer. If the producer already exists, do nothing
    * @param producerInfo the producer
    */
   public async addProducer(producerInfo: ProducerInfo): Promise<void> {
      if (this.producers.has(producerInfo.producer.id)) return;

      this.producers.set(producerInfo.producer.id, producerInfo);

      for (const receiver of this.receivers.values()) {
         // if (receiver.participantId !== producerInfo.participantId) {
         await this.createConsumer(receiver, producerInfo);
      }
   }

   /**
    * Remove a producer and close all consumers if producer was registered
    * @param producerId the producer
    */
   public removeProducer(producerId: string): void {
      const producer = this.producers.get(producerId);
      if (producer) {
         this.producers.delete(producerId);

         for (const receiver of this.receivers.values()) {
            for (const consumer of receiver.consumers.values()) {
               if (consumer.producerId === producerId) {
                  consumer.close();
                  receiver.consumers.delete(consumer.id);
                  // todo: notify consumer about close?
                  break;
               }
            }
         }
      }
   }

   /**
    * add a new receive transport that consumes all producers of this mixer
    * @param connection the connection
    */
   public async addReceiveTransport(connection: Connection): Promise<void> {
      logger.info('add receive connection %s', connection.participantId);

      this.receivers.set(connection.connectionId, connection);

      for (const producerInfo of this.producers.values()) {
         if (producerInfo.participantId !== connection.participantId)
            await this.createConsumer(connection, producerInfo);
      }
   }

   /**
    * remove a receive transport, close all consumers of this mixer
    * @param connectionId the connection
    */
   public removeReceiveTransport(connectionId: string): void {
      const receiver = this.receivers.get(connectionId);
      if (receiver) {
         this.receivers.delete(connectionId);

         for (const consumer of receiver.consumers.values()) {
            if (this.producers.has(consumer.producerId)) {
               consumer.close();
               receiver.consumers.delete(consumer.id);
               // todo: notify consumer about close?
               break;
            }
         }
      }
   }

   private async createConsumer(connection: Connection, { producer, participantId }: ProducerInfo) {
      logger.debug(
         'createConsumer() from %s to %s (producer id: %s)',
         participantId,
         connection.participantId,
         producer.id,
      );

      if (
         !connection.rtpCapabilities ||
         !this.router.canConsume({
            producerId: producer.id,
            rtpCapabilities: connection.rtpCapabilities,
         })
      )
         return;

      const transport = connection.getReceiveTransport();
      // This should not happen.
      if (!transport) {
         logger.warn('createConsumer() | Transport for consuming not found');
         return;
      }

      // Create the Consumer in paused mode.
      let consumer: Consumer;

      try {
         consumer = await transport.consume({
            producerId: producer.id,
            rtpCapabilities: connection.rtpCapabilities,
            paused: false,
         });
      } catch (error) {
         logger.warn('createConsumer() | transport.consume():%o', error);
         return;
      }

      consumer.appData.participantId = participantId;

      // Store the Consumer
      connection.consumers.set(consumer.id, consumer);

      // Set Consumer events.
      consumer.on('transportclose', () => {
         // Remove from its map.
         connection.consumers.delete(consumer.id);
      });

      consumer.on('producerclose', () => {
         // Remove from its map.
         connection.consumers.delete(consumer.id);

         this.signal.sendToConnection(connection.connectionId, 'consumerClosed', { consumerId: consumer.id });
      });

      consumer.on('producerpause', () => {
         this.signal.sendToConnection(connection.connectionId, 'consumerPaused', { consumerId: consumer.id });
      });

      consumer.on('producerresume', () => {
         this.signal.sendToConnection(connection.connectionId, 'consumerResumed', { consumerId: consumer.id });
      });

      consumer.on('score', (score) => {
         this.signal.sendToConnection(connection.connectionId, 'consumerScore', { consumerId: consumer.id, score });
      });

      consumer.on('layerschange', (layers) => {
         this.signal.sendToConnection(connection.connectionId, 'consumerScore', {
            consumerId: consumer.id,
            spatialLayer: layers ? layers.spatialLayer : null,
            temporalLayer: layers ? layers.temporalLayer : null,
         });
      });

      logger.debug('Send newConsumer event to %s', connection.connectionId);
      await this.signal.sendToConnection(connection.connectionId, 'newConsumer', {
         participantId,
         producerId: producer.id,
         id: consumer.id,
         kind: consumer.kind,
         rtpParameters: consumer.rtpParameters,
         type: consumer.type,
         appData: producer.appData,
         producerPaused: consumer.producerPaused,
      });

      this.signal.sendToConnection(connection.connectionId, 'consumerScore', {
         consumerId: consumer.id,
         score: consumer.score,
      });
   }
}
