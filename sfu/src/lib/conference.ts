import { Router } from 'mediasoup/lib/Router';
import { WebRtcTransportOptions, RtpCapabilities } from 'mediasoup/lib/types';
import config from '../config';
import Connection from './connection';
import { CreateTransportRequest, CreateTransportResponse } from './types';

export class Conference {
   private connections: Map<string, Connection> = new Map();

   constructor(private router: Router, public conferenceId: string) {}

   get routerCapabilities(): RtpCapabilities {
      return this.router.rtpCapabilities;
   }

   close() {
      this.router.close();
   }

   addConnection(connection: Connection) {
      // create Consumer
      this.connections.set(connection.connectionId, connection);
   }

   removeConnection(connection: Connection) {
      // wtf

      this.connections.delete(connection.connectionId);
   }

   async createTransport({
      payload: { sctpCapabilities, forceTcp, producing, consuming },
      meta,
   }: CreateTransportRequest): Promise<CreateTransportResponse> {
      const connection = this.connections.get(meta.connectionId);
      if (!connection) throw new Error('Connection was not found');

      const webRtcTransportOptions: WebRtcTransportOptions = {
         ...config.webRtcTransport.options,
         enableSctp: Boolean(sctpCapabilities),
         numSctpStreams: sctpCapabilities?.numStreams,
         appData: { producing, consuming },
      };

      if (forceTcp) {
         webRtcTransportOptions.enableUdp = false;
         webRtcTransportOptions.enableTcp = true;
      }

      const transport = await this.router.createWebRtcTransport(webRtcTransportOptions);
      connection.transport.set(transport.id, transport);

      const { maxIncomingBitrate } = config.webRtcTransport;

      // If set, apply max incoming bitrate limit.
      if (maxIncomingBitrate) {
         try {
            await transport.setMaxIncomingBitrate(maxIncomingBitrate);
         } catch (error) {}
      }

      return {
         meta,
         payload: {
            id: transport.id,
            iceParameters: transport.iceParameters,
            iceCandidates: transport.iceCandidates,
            dtlsParameters: transport.dtlsParameters,
            sctpParameters: transport.sctpParameters,
         },
      };
   }
}
