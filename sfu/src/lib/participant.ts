import Connection from './connection';

export class Participant {
   constructor(public participantId: string) {}

   public connections: Connection[] = [];
}
