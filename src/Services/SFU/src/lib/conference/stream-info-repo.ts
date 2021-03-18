import { Participant } from '../participant';
import { ProducerSource } from '../types';
import { ConferenceMessenger } from './conference-messenger';
import { ConferenceParticipantStreamInfo, ParticipantStreams } from './pub-types';

/**
 * A repository for updating information about streams of the participants
 */
export class StreamInfoRepo {
   constructor(private messenger: ConferenceMessenger) {}

   /**
    * Update all streams, overwrite old info
    * @param participants the participants
    */
   public async updateStreams(participants: IterableIterator<Participant>): Promise<void> {
      const result: ConferenceParticipantStreamInfo = {};

      for (const participant of participants) {
         result[participant.participantId] = this.createParticipantInfo(participant);
      }

      await this.messenger.updateStreams(result);
   }

   private createParticipantInfo(participant: Participant): ParticipantStreams {
      const streams: ParticipantStreams = {
         consumers: {},
         producers: {},
      };

      for (const [type, info] of Object.entries(participant.producers)) {
         if (!info) continue;

         streams.producers[type as ProducerSource] = {
            paused: info.producer.paused,
         };
      }

      for (const connection of participant.connections) {
         for (const [, consumer] of connection.consumers) {
            streams.consumers[consumer.id] = {
               paused: consumer.paused,
               participantId: consumer.appData.participantId,
               loopback: consumer.appData.loopback,
            };
         }
      }

      return streams;
   }
}
