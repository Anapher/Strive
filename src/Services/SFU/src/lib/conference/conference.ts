import _ from 'lodash';
import { Router } from 'mediasoup/lib/Router';
import { Consumer, MediaKind, Producer, RtpCapabilities, WebRtcTransportOptions } from 'mediasoup/lib/types';
import { SuccessOrError } from '../../common-types';
import * as errors from '../../errors';
import Logger from '../../utils/logger';
import Connection from '../connection';
import { Participant } from '../participant';
import { RoomManager } from '../rooms/room-manager';
import { ConferenceRepository } from '../synchronization/conference-repository';
import { ProducerSource } from '../types';
import { ConferenceMessenger } from './conference-messenger';
import {
   ChangeProducerSourceRequest,
   ChangeStreamRequest,
   ConnectTransportRequest,
   CreateTransportRequest,
   CreateTransportResponse,
   InitializeConnectionRequest,
   SetPreferredLayersRequest,
   TransportProduceRequest,
   TransportProduceResponse,
} from './request-types';
import { StreamInfoRepo } from './stream-info-repo';

const logger = new Logger('Conference');

export class Conference {
   private connections: Map<string, Connection> = new Map();
   private roomManager: RoomManager;

   /** participantId -> Participant */
   private participants: Map<string, Participant> = new Map();
   public streamInfoRepo: StreamInfoRepo;

   constructor(
      private router: Router,
      public conferenceId: string,
      private messenger: ConferenceMessenger,
      repo: ConferenceRepository,
      private options: WebRtcTransportOptions,
      private maxIncomingBitrate?: number,
   ) {
      this.roomManager = new RoomManager(conferenceId, messenger, router, repo);
      this.streamInfoRepo = new StreamInfoRepo(messenger, conferenceId);
   }

   get routerCapabilities(): RtpCapabilities {
      return this.router.rtpCapabilities;
   }

   public close(): void {
      logger.info('Close conference %s', this.conferenceId);
      this.router.close();
   }

   public async addConnection(request: InitializeConnectionRequest): Promise<void> {
      const connection = new Connection(
         request.rtpCapabilities,
         request.sctpCapabilities,
         request.connectionId,
         request.participantId,
      );

      // create locally
      this.connections.set(connection.connectionId, connection);

      // search participant, check if it already exists
      let participant = this.participants.get(connection.participantId);
      if (!participant) {
         participant = new Participant(connection.participantId);

         // does not exist, add
         this.participants.set(connection.participantId, participant);
      }

      // add connection
      participant.connections.push(connection);

      // notify room manager
      await this.roomManager.updateParticipant(participant);

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());
   }

   public async removeConnection(connectionId: string): Promise<SuccessOrError> {
      const connection = this.connections.get(connectionId);
      if (!connection) return { success: false, error: errors.connectionNotFound(connectionId) };

      this.connections.delete(connection.connectionId);

      // if that was the last connection of this participant, remove participant
      const participant = this.participants.get(connection.participantId);
      if (participant) {
         // remove connection from participant
         _.remove(participant.connections, (x) => x.connectionId === connection.connectionId);

         for (const [, producer] of connection.producers) {
            producer.close();
            this.removeProducer(producer, participant);
         }
         await this.roomManager.updateParticipant(participant);

         if (participant.connections.length === 0) {
            // remove participant
            this.participants.delete(participant.participantId);
            await this.roomManager.removeParticipant(participant);
         }
      }

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());

      return { success: true };
   }

   public async removeParticipant(participantId: string): Promise<SuccessOrError> {
      logger.info('removeParticipant() | participantId: %s', participantId);

      const participant = this.participants.get(participantId);
      if (!participant) return { success: false, error: errors.participantNotFound(participantId) };

      for (const connection of participant.connections) {
         for (const [, producer] of connection.producers) {
            producer.close();
         }
         for (const [, consumer] of connection.consumers) {
            consumer.close();
         }
         for (const [, transport] of connection.transport) {
            transport.close();
         }
      }

      await this.roomManager.removeParticipant(participant);
      this.participants.delete(participant.participantId);

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());

      return { success: true };
   }

   public async updateParticipant(participantId: string): Promise<SuccessOrError> {
      const participant = this.participants.get(participantId);
      if (!participant) return { success: false, error: errors.participantNotFound(participantId) };

      await this.roomManager.updateParticipant(participant);

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());

      return { success: true };
   }

   /**
    * Change a producer/consumer of the participant. The parameter provides information about the type (consumer|producer),
    * id and action (pause|resume|close)
    */
   public async changeStream({ id, type, action }: ChangeStreamRequest, connectionId: string): Promise<SuccessOrError> {
      const connection = this.connections.get(connectionId);
      if (!connection) {
         return { success: false, error: errors.connectionNotFound(connectionId) };
      }

      let stream: Producer | Consumer | undefined;
      if (type === 'consumer') {
         stream = connection.consumers.get(id);
      } else if (type === 'producer') {
         stream = connection.producers.get(id);
      }

      if (!stream) {
         return { success: false, error: errors.streamNotFound(type, id) };
      }

      if (action === 'pause') {
         await stream.pause();
      } else if (action === 'resume') {
         await stream.resume();
      } else if (action === 'close') {
         stream.close();

         if (type === 'consumer') {
            connection.consumers.delete(id);
         } else {
            connection.producers.delete(id);

            const producer = stream as Producer;
            const participant = this.participants.get(connection.participantId);
            if (participant) {
               this.removeProducer(producer, participant);
               await this.roomManager.updateParticipant(participant);
            }
         }
      }

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());

      return { success: true };
   }

   public async setConsumerLayers(
      { consumerId, layers }: SetPreferredLayersRequest,
      connectionId: string,
   ): Promise<SuccessOrError> {
      const connection = this.connections.get(connectionId);
      if (!connection) {
         return { success: false, error: errors.connectionNotFound(connectionId) };
      }

      const consumer = connection.consumers.get(consumerId);
      if (!consumer) {
         return { success: false, error: errors.streamNotFound('consumer', consumerId) };
      }

      logger.debug('Set prefferred layers to %O', layers);
      await consumer.setPreferredLayers(layers);
      return { success: true };
   }

   /**
    * Change a specific selected producer source of the participant. This may specifically be used by moderators to disable
    * the microphone on certain participants. They can not use changeStream directly as they don't know which connection a
    * producer belongs to
    */
   public async changeProducerSource(
      payload: ChangeProducerSourceRequest,
      participantId: string,
   ): Promise<SuccessOrError> {
      const participant = this.participants.get(participantId);
      if (!participant) {
         return { success: false, error: errors.participantNotFound(participantId) };
      }

      const { source, action } = payload;

      const producerLink = participant.producers[source];
      if (!producerLink) {
         return { success: false, error: errors.producerSourceNotFound(source) };
      }

      const result = await this.changeStream(
         { action, id: producerLink.producer.id, type: 'producer' },
         producerLink.connectionId,
      );

      if (result.success) {
         await this.messenger.notifyProducerChanged(producerLink.connectionId, {
            ...payload,
            producerId: producerLink.producer.id,
         });
      }

      return result;
   }

   /**
    * Create a new producer in an existing transport
    */
   public async transportProduce(
      { transportId, appData, kind, ...producerOptions }: TransportProduceRequest,
      connectionId: string,
   ): Promise<SuccessOrError<TransportProduceResponse>> {
      const connection = this.connections.get(connectionId);
      if (!connection) return { success: false, error: errors.connectionNotFound(connectionId) };

      const participant = this.participants.get(connection.participantId);
      if (!participant) return { success: false, error: errors.participantNotFound(connection.participantId) };

      const transport = connection.transport.get(transportId);
      if (!transport) return { success: false, error: errors.transportNotFound(transportId) };

      const source: ProducerSource = appData.source;
      if (!this.verifyProducerSource(kind, source))
         return { success: false, error: errors.invalidProducerKind(source, kind) };

      appData = { ...appData, participantId: participant.participantId };

      const producer = await transport.produce({
         ...producerOptions,
         kind,
         appData,
      });

      if (participant.producers[source]) {
         participant.producers[source]?.producer.close();
         participant.producers[source] = undefined;
      }

      producer.on('score', (score) => {
         this.messenger.notifyProducerScore(connection.connectionId, { producerId: producer.id, score });
      });

      connection.producers.set(producer.id, producer);
      participant.producers[source] = { producer, connectionId: connection.connectionId };

      await this.roomManager.updateParticipant(participant);

      // update streams
      await this.streamInfoRepo.updateStreams(this.participants.values());

      return { success: true, response: { id: producer.id } };
   }

   /**
    * Connect the transport after initialization
    */
   public async connectTransport(payload: ConnectTransportRequest, connectionId: string): Promise<SuccessOrError> {
      const connection = this.connections.get(connectionId);
      if (!connection) return { success: false, error: errors.connectionNotFound(connectionId) };

      const transport = connection.transport.get(payload.transportId);
      if (!transport) return { success: false, error: errors.transportNotFound(payload.transportId) };

      logger.debug('connectTransport() | participantId: %s', connection.participantId);

      await transport.connect(payload);
      return { success: true };
   }

   /**
    * Initialize a new transport
    */
   public async createTransport(
      { sctpCapabilities, forceTcp, producing, consuming }: CreateTransportRequest,
      connectionId: string,
   ): Promise<SuccessOrError<CreateTransportResponse>> {
      const connection = this.connections.get(connectionId);
      if (!connection) return { success: false, error: errors.connectionNotFound(connectionId) };

      const participant = this.participants.get(connection.participantId);
      if (!participant) return { success: false, error: errors.participantNotFound(connection.participantId) };

      logger.debug('createTransport() | participantId: %s', connection.participantId);

      const webRtcTransportOptions: WebRtcTransportOptions = {
         ...this.options,
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

      // If set, apply max incoming bitrate limit.
      if (this.maxIncomingBitrate) {
         try {
            await transport.setMaxIncomingBitrate(this.maxIncomingBitrate);
         } catch (error) {}
      }

      if (consuming) participant.receiveConnection = connection;

      await this.roomManager.updateParticipant(participant);

      return {
         success: true,
         response: {
            id: transport.id,
            iceParameters: transport.iceParameters,
            iceCandidates: transport.iceCandidates,
            dtlsParameters: transport.dtlsParameters,
            sctpParameters: transport.sctpParameters,
         },
      };
   }

   private removeProducer(producer: Producer, participant: Participant): void {
      for (const [k, activeProducer] of Object.entries(participant.producers)) {
         if (activeProducer?.producer.id === producer.id) {
            participant.producers[k as ProducerSource] = undefined;
         }
      }
   }

   private verifyProducerSource(kind: MediaKind, source: ProducerSource): boolean {
      if (source === 'mic' && kind === 'audio') return true;
      if (source === 'screen' && kind === 'video') return true;
      if (source === 'webcam' && kind === 'video') return true;

      if (source === 'loopback-mic' && kind === 'audio') return true;
      if (source === 'loopback-webcam' && kind === 'video') return true;
      if (source === 'loopback-screen' && kind === 'video') return true;

      return false;
   }
}
