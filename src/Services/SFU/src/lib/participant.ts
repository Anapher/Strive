import Connection from './connection';
import { ProducerLink, ProducerSource } from './types';

export class Participant {
   constructor(public participantId: string) {}

   public connections: Connection[] = [];
   public producers: { [key in ProducerSource]?: ProducerLink } = {};
   public receiveConnection: Connection | undefined;
}
