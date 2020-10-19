import { Redis } from 'ioredis';
import { Router } from 'mediasoup/lib/types';
import * as redisKeys from './pader-conference/redis-keys';
import { Participant } from './participant';
import Room from './room';
import { ISignalWrapper } from './signal-wrapper';

export class RoomManager {
   private participantToRoomKey: string;

   constructor(conferenceId: string, private signal: ISignalWrapper, private router: Router, private redis: Redis) {
      this.participantToRoomKey = redisKeys.participantToRoom(conferenceId);
   }

   /** roomId -> Room */
   private roomMap: Map<string, Room> = new Map();

   /** participantId -> roomId */
   private participantToRoom = new Map<string, string>();

   public async updateParticipant(participant: Participant): Promise<void> {
      const roomId = await this.getParticipantRoom(participant.participantId);
      if (!roomId) return;

      // get the room or create a new one
      let room = this.roomMap.get(roomId);
      if (!room) {
         room = new Room(roomId, this.signal, this.router, this.redis);
         this.roomMap.set(roomId, room);
      }

      const currentRoomId = this.participantToRoom.get(participant.participantId);
      if (currentRoomId && roomId !== currentRoomId) {
         // room switch, remove from current room
         const currentRoom = this.roomMap.get(currentRoomId);
         if (currentRoom) {
            currentRoom.leave(participant);

            if (currentRoom.participants.size === 0) {
               this.closeRoom(currentRoom);
            }
         }
      }

      if (roomId !== currentRoomId) {
         // join the new room
         room.join(participant);
      } else {
         // just update the participant in the room
         room.updateParticipant(participant);
      }
   }

   public async removeParticipant(participant: Participant): Promise<void> {
      const roomId = await this.getParticipantRoom(participant.participantId);
      if (!roomId) return;

      const room = this.roomMap.get(roomId);
      if (room) {
         room.leave(participant);

         if (room.participants.size === 0) {
            this.closeRoom(room);
         }
      }
   }

   private closeRoom(room: Room) {
      this.roomMap.delete(room.id);
   }

   private async getParticipantRoom(participantId: string): Promise<string | null> {
      return await this.redis.hget(this.participantToRoomKey, participantId);
   }
}
