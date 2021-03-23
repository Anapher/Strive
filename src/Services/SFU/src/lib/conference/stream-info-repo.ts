import { Participant } from '../participant';
import { ProducerSource } from '../types';
import { ConferenceMessenger } from './conference-messenger';
import { ConferenceParticipantStreamInfo, ParticipantStreams } from './pub-types';

/**
 * A repository for updating information about streams of the participants
 */
export class StreamInfoRepo {
   private isFrozen = false;
   private latestValue: ConferenceParticipantStreamInfo | undefined;

   constructor(private messenger: ConferenceMessenger, private conferenceId: string) {}

   /**
    * Freeze the stream repo, meaning that any new values will be cached instead of being sent.
    * If the return value callback is invoked, the latest value will be sent.
    * @returns Release the frozen state
    */
   public freeze(): () => Promise<void> {
      this.isFrozen = true;

      return async () => {
         if (!this.isFrozen) return;

         this.isFrozen = false;
         if (this.latestValue) {
            await this.messenger.updateStreams(this.latestValue, this.conferenceId);
            this.latestValue = undefined;
         }
      };
   }

   /**
    * Update all streams, overwrite old info
    * @param participants the participants
    */
   public async updateStreams(participants: IterableIterator<Participant>): Promise<void> {
      const result: ConferenceParticipantStreamInfo = {};

      for (const participant of participants) {
         result[participant.participantId] = this.createParticipantInfo(participant);
      }

      if (this.isFrozen) {
         this.latestValue = result;
         return;
      }

      await this.messenger.updateStreams(result, this.conferenceId);
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
