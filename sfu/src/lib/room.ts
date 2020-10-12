import _ from 'lodash';
import { Consumer, Producer, Router } from 'mediasoup/lib/types';
import Logger from './Logger';
import { Participant } from './participant';
import { ISignalWrapper } from './signal-wrapper';

const logger = new Logger('Room');

/** each participant is in excactly one room. The room determines to which participants the media is sent */
export default class Room {
   constructor(public id: string, private signal: ISignalWrapper, private router: Router) {}

   public participants: Participant[] = [];

   join(participant: Participant): void {
      logger.info('join() | participantId: %s | roomId: %s', participant.participantId, this.id);

      for (const producerParticipant of this.participants) {
         for (const connection of producerParticipant.connections) {
            for (const [_, producer] of connection.producers) {
               this.createConsumer(producerParticipant.participantId, participant, producer, this.router);
            }
         }
      }

      this.participants.push(participant);
   }

   leave(participant: Participant): void {
      logger.info('leave() | participantId: %s | roomId: %s', participant.participantId, this.id);

      for (const connection of participant.connections) {
         for (const consumer of connection.consumers.values()) {
            consumer.close();
         }
      }

      _.remove(this.participants, (x) => x.participantId === participant.participantId);
   }

   produce(participantId: string, producer: Producer): void {
      for (const consumerParticipant of this.participants) {
         if (consumerParticipant.participantId === participantId) continue;
         this.createConsumer(participantId, consumerParticipant, producer, this.router);
      }
   }

   private async createConsumer(
      producerParticipantId: string,
      consumerParticipant: Participant,
      producer: Producer,
      router: Router,
   ) {
      logger.debug(
         'createConsumer() from %s to %s (producer id: %s)',
         producerParticipantId,
         consumerParticipant.participantId,
         producer.id,
      );

      const consumerConn = consumerParticipant.connections[0]; // TODO handle multiple connections

      if (
         !consumerConn.rtpCapabilities ||
         !router.canConsume({
            producerId: producer.id,
            rtpCapabilities: consumerConn.rtpCapabilities,
         })
      )
         return;

      const transport = Array.from(consumerConn.transport.values()).find((t) => t.appData.consuming);
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
            rtpCapabilities: consumerConn.rtpCapabilities,
            paused: false,
         });
      } catch (error) {
         logger.warn('createConsumer() | transport.consume():%o', error);

         return;
      }

      // Store the Consumer
      consumerConn.consumers.set(consumer.id, consumer);

      // Set Consumer events.
      consumer.on('transportclose', () => {
         // Remove from its map.
         consumerConn.consumers.delete(consumer.id);
      });

      consumer.on('producerclose', () => {
         // Remove from its map.
         consumerConn.consumers.delete(consumer.id);

         this.signal.sendToConnection(consumerConn.connectionId, 'consumerClosed', { consumerId: consumer.id });
      });

      consumer.on('producerpause', () => {
         this.signal.sendToConnection(consumerConn.connectionId, 'consumerPaused', { consumerId: consumer.id });
      });

      consumer.on('producerresume', () => {
         this.signal.sendToConnection(consumerConn.connectionId, 'consumerResumed', { consumerId: consumer.id });
      });

      consumer.on('score', (score) => {
         this.signal.sendToConnection(consumerConn.connectionId, 'consumerScore', { consumerId: consumer.id, score });
      });

      consumer.on('layerschange', (layers) => {
         this.signal.sendToConnection(consumerConn.connectionId, 'consumerScore', {
            consumerId: consumer.id,
            spatialLayer: layers ? layers.spatialLayer : null,
            temporalLayer: layers ? layers.temporalLayer : null,
         });
      });

      logger.debug('Send newConsumer event to %s', consumerConn.connectionId);
      await this.signal.sendToConnection(consumerConn.connectionId, 'newConsumer', {
         participantId: producerParticipantId,
         producerId: producer.id,
         id: consumer.id,
         kind: consumer.kind,
         rtpParameters: consumer.rtpParameters,
         type: consumer.type,
         appData: producer.appData,
         producerPaused: consumer.producerPaused,
      });

      this.signal.sendToConnection(consumerConn.connectionId, 'consumerScore', {
         consumerId: consumer.id,
         score: consumer.score,
      });
   }
}
