import { Router } from 'mediasoup/lib/Router';
import Logger from '../../utils/logger';
import { ConferenceMessenger } from '../conference/conference-messenger';
import { Participant } from '../participant';
import { ConferenceRepository } from '../synchronization/conference-repository';
import { ProducerSource } from '../types';
import Room from './room';

const LOOPBACK_PRODUCER_SOURCES: ProducerSource[] = ['loopback-mic', 'loopback-webcam', 'loopback-screen'];

const logger = new Logger('LoopbackManager');

// bascially, this class creates for every participant a single room that ignores permissions
// and adds the custom property 'loopback=true' to the appData of every consumer.
// As the participant is the only one who can join the room, every room in the loopbackMap has
// exactly one participant

/**
 * A manager class for participant loop back
 */
export class LoopbackManager {
   private loopbackMap = new Map<string, Room>();

   constructor(
      private signal: ConferenceMessenger,
      private router: Router,
      private repo: ConferenceRepository,
      private conferenceId: string,
   ) {}

   /**
    * Update the participant, create or remove the looopback room depending whether the participant has active loopback sources
    * @param participant the participant
    */
   public async updateParticipant(participant: Participant): Promise<void> {
      // if the participant has no loopback sources
      if (!LoopbackManager.participantHasLoopbackSources(participant)) {
         await this.disableLoopback(participant);
         return;
      }

      const room = await this.enableLoopback(participant);
      await room.updateParticipant(participant);
   }

   /**
    * Enable loopback streams for a participant
    * @param participant the participant
    */
   private async enableLoopback(participant: Participant): Promise<Room> {
      let room = this.loopbackMap.get(participant.participantId);
      if (!room) {
         logger.info('Enable loopback for %s', participant.participantId);

         const roomId = `loopback-${participant.participantId}`;
         room = new Room(roomId, this.signal, this.router, this.repo, this.conferenceId, LOOPBACK_PRODUCER_SOURCES);
         this.loopbackMap.set(participant.participantId, room);

         await room.join(participant);
      }

      return room;
   }

   /**
    * Disable loopback streams for a participant
    * @param participant the participant
    */
   public async disableLoopback(participant: Participant): Promise<void> {
      const room = this.loopbackMap.get(participant.participantId);
      if (room) {
         logger.info('Disable loopback for %s', participant.participantId);

         await room.leave(participant);
         this.loopbackMap.delete(participant.participantId);
      }
   }

   private static participantHasLoopbackSources(participant: Participant) {
      return Object.entries(participant.producers).find(
         ([source, producer]) => producer !== undefined && LOOPBACK_PRODUCER_SOURCES.includes(source as ProducerSource),
      );
   }
}
