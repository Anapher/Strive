import { Producer } from 'mediasoup/lib/types';
import Connection from './connection';

export class Participant {
   constructor(public participantId: string) {}

   public connections: Connection[] = [];

   producers: { [key in ProducerSource]?: ProducerLink } = {};

   public getReceiveConnection(): Connection | undefined {
      // find first consuming transport
      for (const conn of this.connections) {
         const receiveTransport = conn.getReceiveTransport();
         if (receiveTransport) return conn;
      }

      return undefined;
   }
}

export const producerSources = [
   'mic',
   'webcam',
   'screen',
   'loopback-mic',
   'loopback-webcam',
   'loopback-screen',
] as const;

export type ProducerSource = typeof producerSources[number];

export type ProducerLink = {
   connectionId: string;
   producer: Producer;
};
