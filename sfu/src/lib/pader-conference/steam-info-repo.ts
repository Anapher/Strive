import { ConferenceParticipantStreamInfo } from './../types';
import { Redis } from 'ioredis';
import { Participant } from '../participant';
import { ParticipantStreams } from '../types';
import { conferenceStreams } from './redis-keys';
import { channels } from './redis-channels';

/**
 * A repository for creating the stream information in database
 */
export class StreamInfoRepo {
   private redisKey: string;
   private channelKey: string;

   constructor(private redis: Redis, conferenceId: string) {
      this.redisKey = conferenceStreams(conferenceId);
      this.channelKey = channels.streamsChanged.getName(conferenceId);
   }

   /**
    * Update all streams in database, overwrite old info
    * @param participants the participants
    */
   public async updateStreams(participants: IterableIterator<Participant>): Promise<void> {
      const result: ConferenceParticipantStreamInfo = {};

      for (const participant of participants) {
         result[participant.participantId] = this.createParticipantInfo(participant);
      }

      await this.redis.set(this.redisKey, JSON.stringify(result));
      await this.redis.publish(this.channelKey, 'null');
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
