import { Router } from 'mediasoup/lib/types';
import { ConferenceMessenger } from '../conference/conference-messenger';
import { Participant } from '../participant';
import { ConferenceRepository } from '../synchronization/conference-repository';
import { ProducerSource } from '../types';
import { LoopbackManager } from './loopback-manager';
import Room from './room';

const DEFAULT_ROOM_SOURCES: ProducerSource[] = ['mic', 'webcam', 'screen'];

/**
 * The room manager opens and closes media rooms and moves participant to the correct room
 */
export class RoomManager {
   constructor(
      private conferenceId: string,
      private signal: ConferenceMessenger,
      private router: Router,
      private repo: ConferenceRepository,
   ) {
      this.loopbackManager = new LoopbackManager(signal, router, repo, conferenceId);
   }

   private loopbackManager: LoopbackManager;

   /** roomId -> Room */
   private roomMap: Map<string, Room> = new Map();

   /** participantId -> roomId */
   private participantToRoom = new Map<string, string>();

   public async updateParticipant(participant: Participant): Promise<void> {
      // loopback is independent from the room
      await this.loopbackManager.updateParticipant(participant);

      const newRoomId = await this.getParticipantRoom(participant.participantId);
      const currentRoomId = this.participantToRoom.get(participant.participantId);

      if (newRoomId == currentRoomId) {
         // no room change, just update the participant if it belongs to a room
         if (currentRoomId) {
            const room = this.roomMap.get(currentRoomId);
            if (!room) throw new Error('The room is set but does not exist');

            await room.updateParticipant(participant);
         }

         return;
      }

      if (currentRoomId) {
         // room switch, remove from current room
         const currentRoom = this.roomMap.get(currentRoomId);
         if (currentRoom) {
            await currentRoom.leave(participant);

            if (currentRoom.participants.size === 0) {
               this.closeRoom(currentRoom);
            }
         }
      }

      if (newRoomId) {
         // get the room or create a new one
         let room = this.roomMap.get(newRoomId);
         if (!room) {
            room = new Room(newRoomId, this.signal, this.router, this.repo, this.conferenceId, DEFAULT_ROOM_SOURCES);
            this.roomMap.set(newRoomId, room);
         }

         await room.join(participant);
         this.participantToRoom.set(participant.participantId, newRoomId);
      }
   }

   public async removeParticipant(participant: Participant): Promise<void> {
      await this.loopbackManager.disableLoopback(participant);

      const roomId = await this.getParticipantRoom(participant.participantId);
      if (!roomId) return;

      const room = this.roomMap.get(roomId);
      if (room) {
         await room.leave(participant);
         this.participantToRoom.delete(participant.participantId);

         if (room.participants.size === 0) {
            this.closeRoom(room);
         }
      }
   }

   private closeRoom(room: Room) {
      this.roomMap.delete(room.id);
   }

   private async getParticipantRoom(participantId: string): Promise<string | undefined> {
      const conference = await this.repo.getConference(this.conferenceId);
      return conference.participantToRoom.get(participantId);
   }
}
