import { Consumer, ConsumerLayers, Producer, Router } from 'mediasoup/lib/types';
import Logger from '../../utils/logger';
import { ConferenceMessenger } from '../conference/conference-messenger';
import Connection from '../connection';

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

   /**
    * Initialize a new MediasoupMixer
    * @param router the router the mixer should depend on
    * @param signal the signal wrapper for communication
    */
   constructor(private router: Router, private signal: ConferenceMessenger) {}

   /**
    * Add a new producer and consume it by all receivers of this mixer. If the producer already exists, do nothing
    * @param producerInfo the producer
    */
   public async addProducer(producerInfo: ProducerInfo): Promise<void> {
      if (this.producers.has(producerInfo.producer.id)) return;

      logger.info('add producer from %s', producerInfo.participantId);

      this.producers.set(producerInfo.producer.id, producerInfo);

      for (const receiver of this.receivers.values()) {
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

                  this.signal.notifyConsumerClosed(receiver.connectionId, { consumerId: consumer.id });
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
         await this.createConsumer(connection, producerInfo);
      }
   }

   /**
    * remove a receive transport, close all consumers of this mixer
    * @param connectionId the connection
    */
   public async removeReceiveTransport(connectionId: string): Promise<void> {
      const receiver = this.receivers.get(connectionId);
      if (receiver) {
         this.receivers.delete(connectionId);

         for (const consumer of receiver.consumers.values()) {
            if (this.producers.has(consumer.producerId)) {
               consumer.close();
               receiver.consumers.delete(consumer.id);

               await this.signal.notifyConsumerClosed(receiver.connectionId, { consumerId: consumer.id });
            }
         }
      }
   }

   private async createConsumer(connection: Connection, { producer, participantId }: ProducerInfo): Promise<void> {
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
            paused: true,
         });
      } catch (error) {
         logger.warn('createConsumer() | transport.consume():%o', error);
         return;
      }

      consumer.appData.participantId = participantId;
      consumer.appData.source = producer.appData.source;

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

         this.signal.notifyConsumerClosed(connection.connectionId, { consumerId: consumer.id });
      });

      consumer.on('producerpause', () => {
         this.signal.notifyConsumerPaused(connection.connectionId, { consumerId: consumer.id });
      });

      consumer.on('producerresume', () => {
         this.signal.notifyConsumerResumed(connection.connectionId, { consumerId: consumer.id });
      });

      consumer.on('score', (score) => {
         this.signal.notifyConsumerScore(connection.connectionId, { consumerId: consumer.id, score });
      });

      consumer.on('layerschange', (layers: ConsumerLayers | undefined) => {
         if (!layers) return;

         this.signal.notifyConsumerLayersChanged(connection.connectionId, {
            consumerId: consumer.id,
            layers,
         });
      });

      logger.debug('Send newConsumer event to %s', connection.connectionId);
      await this.signal.notifyConsumerCreated(connection.connectionId, {
         participantId,
         producerId: producer.id,
         id: consumer.id,
         kind: consumer.kind,
         rtpParameters: consumer.rtpParameters,
         type: consumer.type,
         appData: producer.appData,
         producerPaused: consumer.producerPaused,
      });

      this.signal.notifyConsumerScore(connection.connectionId, {
         consumerId: consumer.id,
         score: consumer.score,
      });
   }
}
