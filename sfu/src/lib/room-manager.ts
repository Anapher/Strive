import _ from 'lodash';
import { Router } from 'mediasoup/lib/types';
import Connection from './connection';
import { Participant } from './participant';
import Room from './room';
import { ISignalWrapper } from './signal-wrapper';

export class RoomManager {
   constructor(signal: ISignalWrapper, router: Router) {
      this.rooms = [(this.defaultRoom = new Room('master', signal, router))];
   }

   /** participantId -> Participant */
   private participants: Map<string, Participant> = new Map();

   /** participantId -> Room */
   private roomMap: Map<string, Room> = new Map();

   public defaultRoom: Room;
   public rooms: Room[];

   public addConnection(connection: Connection): void {
      // search participant, check if it already exists
      let participant = this.participants.get(connection.participantId);
      if (!participant) {
         participant = new Participant(connection.participantId);
         this.participants.set(connection.participantId, participant);
      }

      // add connection
      participant.connections.push(connection);

      // check if the participant is already in a room
      let room = this.roomMap.get(connection.participantId);
      if (!room) {
         // else add to default room
         room = this.defaultRoom;
         this.roomMap.set(connection.participantId, room);
         room.join(participant);
      }
   }

   public removeConnection(connection: Connection): void {
      const room = this.roomMap.get(connection.participantId);
      if (room) {
         const participant = this.participants.get(connection.participantId);
         if (participant) {
            _.remove(participant.connections, (x) => x.connectionId === connection.connectionId);
            if (participant.connections.length === 0) {
               room.leave(participant);
               this.roomMap.delete(connection.participantId);
               this.participants.delete(participant.participantId);
            }
         }
      }

      for (const consumer of connection.consumers.values()) {
         consumer.close();
      }
   }

   public getRoom(participantId: string): Room | undefined {
      return this.roomMap.get(participantId);
   }
}
