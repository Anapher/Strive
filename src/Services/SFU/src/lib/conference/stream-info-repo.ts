import { Participant } from '../participant';
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
      const selectedEntries = Object.entries(participant.producers);

      const info: ParticipantStreams = { consumers: {}, producers: {} };

      for (const connection of participant.connections) {
         for (const [, consumer] of connection.consumers) {
            info.consumers[consumer.id] = {
               paused: consumer.paused,
               participantId: consumer.appData.participantId,
               loopback: consumer.appData.loopback,
            };
         }

         for (const [, producer] of connection.producers) {
            const selected = selectedEntries.find(([, x]) => x?.producer.id === producer.id);

            info.producers[producer.id] = {
               paused: producer.paused,
               selected: !!selected,
               kind: producer.appData.source,
            };
         }
      }

      return info;
   }
}
