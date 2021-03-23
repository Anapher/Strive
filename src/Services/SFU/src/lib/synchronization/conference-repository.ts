import Logger from '../../utils/logger';
import { ConferenceInfo, ConferenceInfoUpdate } from '../types';
import { ConferenceManagementClient } from './conference-management-client';
import PubSubMessenger from './pub-sub-messenger';
import RabbitMqConn, { RabbitChannel } from '../../rabbitmq/rabbit-mq-conn';
import { SyncConferenceEventMessage, SynchronizedConference } from './synchronized-conference';
import _ from 'lodash';

const logger = new Logger();

type MessageHandler = (message: SyncConferenceEventMessage) => void;

export class ConferenceRepository {
   private cachedConferences = new Map<string, SynchronizedConference>();
   private messageHandlers = new Map<string, MessageHandler[]>();

   private cachedChannel: RabbitChannel | undefined;
   private pubSubMessenger: PubSubMessenger;

   constructor(private client: ConferenceManagementClient, private connection: RabbitMqConn) {
      this.pubSubMessenger = new PubSubMessenger(connection);
      connection.on('reset', this.init.bind(this));
   }

   /**
    * add the conference to the watched conferences and return the current snapshot
    */
   public async getConference(conferenceId: string): Promise<ConferenceInfo> {
      await this.init();

      logger.debug('Request conference %s', conferenceId);

      let cached = this.cachedConferences.get(conferenceId);
      if (!cached) {
         cached = await this.createCachedConference(conferenceId);
         this.cachedConferences.set(conferenceId, cached);
      } else {
         logger.debug('Conference is cached, return data');
      }

      return cached.conferenceInfo;
   }

   /**
    * add a handler for the conference update
    */
   public async addMessageHandler(conferenceId: string, handler: MessageHandler): Promise<void> {
      await this.getConference(conferenceId);

      const synchronizedConference = this.cachedConferences.get(conferenceId);
      if (!synchronizedConference) throw new Error('The conference is not cached, somehow getConference failed.');

      synchronizedConference.on('message', handler);

      let handlers = this.messageHandlers.get(conferenceId);
      if (!handlers) this.messageHandlers.set(conferenceId, (handlers = []));
      handlers.push(handler);
   }

   /**
    * remove a handler for the conference update
    */
   public removeMessageHandler(conferenceId: string, handler: MessageHandler): void {
      const synchronizedConference = this.cachedConferences.get(conferenceId);
      if (synchronizedConference) {
         synchronizedConference.off('message', handler);
      }

      const handlers = this.messageHandlers.get(conferenceId);
      if (handlers) {
         this.messageHandlers.set(
            conferenceId,
            handlers.filter((x) => x !== handler),
         );
      }
   }

   private async init(): Promise<void> {
      const channel = await this.connection.getChannel();
      if (channel !== this.cachedChannel) {
         logger.debug('Initialize rabbit mq connection');

         const oldConferences = new Map(this.cachedConferences);

         this.cachedConferences.clear();
         this.cachedChannel = channel;

         await this.restoreHandlers(oldConferences);
      }
   }

   private async createCachedConference(conferenceId: string): Promise<SynchronizedConference> {
      // first, subscribe to events for this conference. This is important to prevent race conditions
      // if the conference changes after we fetched the conference but before we begin processing messages
      const syncConference = await this.pubSubMessenger.createSynchronizedConference(conferenceId);
      const current = await this.client.fetchConference(conferenceId);

      syncConference.initialize(current);
      return syncConference;
   }

   private async restoreHandlers(oldConferences: Map<string, SynchronizedConference>): Promise<void> {
      for (const [conferenceId, handlers] of this.messageHandlers.entries()) {
         await this.getConference(conferenceId);
         const syncConference = this.cachedConferences.get(conferenceId)!;

         const oldData = oldConferences.get(conferenceId)!.conferenceInfo;
         const oldParticipants = getConferenceInfoParticipants(oldData);

         const currentValue = syncConference.conferenceInfo;
         const newParticipants = getConferenceInfoParticipants(currentValue);
         const removedParticipants = oldParticipants.filter((x) => !newParticipants.includes(x));

         for (const handler of handlers) {
            handler({ type: 'conferenceInfoUpdated', update: { ...currentValue, removedParticipants } });
            syncConference.on('message', handler);
         }
      }
   }
}

function getConferenceInfoParticipants(info: ConferenceInfo): string[] {
   return _(Array.from(info.participantPermissions.keys()))
      .concat(Array.from(info.participantToRoom.keys()))
      .uniq()
      .value();
}
