import { Consumer, Producer, RtpCapabilities, SctpCapabilities, Transport } from 'mediasoup/lib/types';

export default class Connection {
   constructor(
      public rtpCapabilities: RtpCapabilities,
      public sctpCapabilities: SctpCapabilities,
      /** the connection id (from SignalR) */
      public connectionId: string,

      /** participant id */
      public participantId: string,

      /**  true if the client is joined so he can receive and produce media, false if it is still creating transports and currently joining */
      public joined: boolean = false,
   ) {}

   public transport: Map<string, Transport> = new Map();
   public producers: Map<string, Producer> = new Map();
   public consumers: Map<string, Consumer> = new Map();

   public getReceiveTransport(): Transport | undefined {
      // find first consuming transport
      for (const [, transport] of this.transport) {
         if (transport.appData.consuming) return transport;
      }

      return undefined;
   }
}
