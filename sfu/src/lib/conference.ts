import { Router } from 'mediasoup/lib/Router';
import { RtpCapabilities, WebRtcTransportOptions } from 'mediasoup/lib/types';
import config from '../config';
import Connection from './connection';
import { RoomManager } from './room-manager';
import { ISignalWrapper } from './signal-wrapper';
import {
   ConnectTransportRequest,
   CreateTransportRequest,
   CreateTransportResponse,
   TransportProduceRequest,
   TransportProduceResponse,
} from './types';

export class Conference {
   private connections: Map<string, Connection> = new Map();
   private roomManager: RoomManager;

   constructor(private router: Router, public conferenceId: string, private signal: ISignalWrapper) {
      this.roomManager = new RoomManager(signal, router);
   }

   get routerCapabilities(): RtpCapabilities {
      return this.router.rtpCapabilities;
   }

   close(): void {
      this.router.close();
   }

   addConnection(connection: Connection): void {
      // create Consumer
      this.connections.set(connection.connectionId, connection);
      this.roomManager.addConnection(connection);
   }

   removeConnection(connectionId: string): void {
      const connection = this.connections.get(connectionId);
      if (connection) {
         this.roomManager.removeConnection(connection);
         this.connections.delete(connection.connectionId);
      }
   }

   async transportProduce({
      payload: { transportId, appData, ...producerOptions },
      meta,
   }: TransportProduceRequest): Promise<TransportProduceResponse> {
      const connection = this.connections.get(meta.connectionId);
      if (!connection) throw new Error('Connection was not found');

      const transport = connection.transport.get(transportId);
      if (!transport) throw new Error(`transport with id "${transportId}" not found`);

      appData = { ...appData, participantId: meta.participantId };

      const producer = await transport.produce({
         ...producerOptions,
         appData,
         // keyFrameRequestDelay: 5000
      });

      producer.on('score', (score) => {
         this.signal.sendToConnection(connection.connectionId, 'producerScore', { producerId: producer.id, score });
      });

      connection.producers.set(producer.id, producer);
      const room = this.roomManager.getRoom(connection.participantId);
      if (room) {
         room.produce(connection.participantId, producer);
      }

      return { id: producer.id };
   }

   async connectTransport({ payload, meta }: ConnectTransportRequest): Promise<void> {
      const connection = this.connections.get(meta.connectionId);
      if (!connection) throw new Error('Connection was not found');

      const transport = connection.transport.get(payload.transportId);
      if (!transport) throw new Error(`transport with id "${payload.transportId}" not found`);

      await transport.connect(payload);
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
         id: transport.id,
         iceParameters: transport.iceParameters,
         iceCandidates: transport.iceCandidates,
         dtlsParameters: transport.dtlsParameters,
         sctpParameters: transport.sctpParameters,
      };
   }
}
