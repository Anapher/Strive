import { Producer } from 'mediasoup/lib/types';
import Connection from './connection';

export class Participant {
   constructor(public participantId: string) {}

   public connections: Connection[] = [];

   producers: { [key in ProducerSource]: Producer | undefined } = {
      mic: undefined,
      webcam: undefined,
      screen: undefined,
   };

   public getReceiveConnection(): Connection | undefined {
      // find first consuming transport
      for (const conn of this.connections) {
         const receiveTransport = conn.getReceiveTransport();
         if (receiveTransport) return conn;
      }

      return undefined;
   }
}

export type ProducerSource = 'mic' | 'webcam' | 'screen';

export const producerSources: ProducerSource[] = ['mic', 'webcam', 'screen'];
