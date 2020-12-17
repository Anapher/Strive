import { SuccessOrError } from './communication-types';
import ConferenceManager from './conference-manager';
import Connection from './connection';
import Logger from './logger';
import {
   ChangeStreamRequest,
   ConnectionMessage,
   ConnectTransportRequest,
   CreateTransportRequest,
   CreateTransportResponse,
   InitializeConnectionRequest,
   TransportProduceRequest,
   TransportProduceResponse,
} from './types';
import * as errors from './errors';

const logger = new Logger('RedisMessageProcessor');

export class RedisMessageProcessor {
   constructor(private conferenceManager: ConferenceManager) {}

   public initializeConnection(request: InitializeConnectionRequest): SuccessOrError<void> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      logger.debug('Initialize connection in conference %s', conference.conferenceId);

      const connection = new Connection(
         request.payload.rtpCapabilities,
         request.payload.sctpCapabilities,
         request.meta.connectionId,
         request.meta.participantId,
      );

      conference.addConnection(connection);
      return { success: true };
   }

   public async createTransport(request: CreateTransportRequest): Promise<SuccessOrError<CreateTransportResponse>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.createTransport(request);
   }

   public async connectTransport(request: ConnectTransportRequest): Promise<SuccessOrError<void>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.connectTransport(request);
   }

   public async transportProduce(request: TransportProduceRequest): Promise<SuccessOrError<TransportProduceResponse>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.transportProduce(request);
   }

   public async roomSwitched(request: ConnectionMessage<any>): Promise<SuccessOrError<void>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.roomSwitched(request);
   }

   public async changeStream(request: ChangeStreamRequest): Promise<SuccessOrError<void>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.changeStream(request);
   }

   public async clientDisconnected(request: ConnectionMessage<undefined>): Promise<SuccessOrError<void>> {
      const conference = this.conferenceManager.getConference(request.meta.conferenceId);
      if (!conference) return { success: false, error: errors.conferenceNotFound(request.meta.conferenceId) };

      return await conference.removeConnection(request.meta.connectionId);
   }
}
