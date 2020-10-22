import { newConferences } from './pader-conference/redis-keys';
import { Redis } from 'ioredis';
import ConferenceManager from './conference-manager';
import Connection from './connection';
import Logger from './logger';
import {
   ChangeStreamRequest,
   ConferenceInfo,
   ConnectionMessage,
   ConnectTransportRequest,
   CreateTransportRequest,
   CreateTransportResponse,
   InitializeConnectionRequest,
   TransportProduceRequest,
   TransportProduceResponse,
} from './types';

const logger = new Logger('RedisMessageProcessor');

export class RedisMessageProcessor {
   constructor(
      private redis: Redis,
      private conferenceManager: ConferenceManager,
      private initializeConference: (conferenceInfo: ConferenceInfo) => Promise<void>,
   ) {}

   public initializeConnection(request: InitializeConnectionRequest): void {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);

      logger.debug('Initialize connection in conference %s', conference.conferenceId);

      const connection = new Connection(
         request.payload.rtpCapabilities,
         request.payload.sctpCapabilities,
         request.meta.connectionId,
         request.meta.participantId,
      );

      conference.addConnection(connection);
   }

   public async createTransport(request: CreateTransportRequest): Promise<CreateTransportResponse> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      return await conference.createTransport(request);
   }

   public async connectTransport(request: ConnectTransportRequest): Promise<void> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      await conference.connectTransport(request);
   }

   public async transportProduce(request: TransportProduceRequest): Promise<TransportProduceResponse> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      return await conference.transportProduce(request);
   }

   public async roomSwitched(request: ConnectionMessage<any>): Promise<void> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      return await conference.roomSwitched(request);
   }

   public async changeStream(request: ChangeStreamRequest): Promise<void> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      return await conference.changeStream(request);
   }

   public async clientDisconnected(request: ConnectionMessage<undefined>): Promise<void> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      await conference.removeConnection(request.meta.connectionId);
   }

   public async newConferenceCreated(): Promise<void> {
      const conferenceStr = await this.redis.lpop(newConferences);
      if (conferenceStr) {
         const conferenceInfo: ConferenceInfo = JSON.parse(conferenceStr);
         this.initializeConference(conferenceInfo);
      }
   }
}
