import Logger from '../../utils/logger';
import { ConferenceInfo } from '../types';
import { ConferenceManagementClient } from './conference-management-client';
import PubSubMessenger from './pub-sub-messenger';
import RabbitMqConn, { RabbitChannel } from './rabbit-mq-conn';
import { SynchronizedConference } from './synchronized-conference';

const logger = new Logger();

export class ConferenceRepository {
   private cachedConferences = new Map<string, SynchronizedConference>();
   private cachedChannel: RabbitChannel | undefined;
   private pubSubMessenger: PubSubMessenger;

   constructor(private client: ConferenceManagementClient, private connection: RabbitMqConn) {
      this.pubSubMessenger = new PubSubMessenger(connection);
   }

   private async init(): Promise<void> {
      const channel = await this.connection.getChannel();
      if (channel !== this.cachedChannel) {
         logger.debug('Initialize rabbit mq connection');

         // the connection changed (either first use or we lost connection). Clear all cached conferences
         // as they may have missed some events
         this.cachedConferences.clear();
         this.cachedChannel = channel;
      }
   }

   /**
    * add the conference to the watched conferences and return the current snapshot
    * @param conferenceId the conference id
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

   public async addMessageHandler(conferenceId: string, handler: (x: any) => void): Promise<void> {
      await this.getConference(conferenceId);

      const synchronizedConference = this.cachedConferences.get(conferenceId);
      if (!synchronizedConference) throw new Error('The conference is not cached, somehow getConference failed.');

      synchronizedConference.on('message', handler);
   }

   private async createCachedConference(conferenceId: string): Promise<SynchronizedConference> {
      // first, subscribe to events for this conference. This is important to prevent race conditions
      // if the conference changes after we fetched the conference but before we begin processing messages
      const factory = await this.pubSubMessenger.createSynchronizedConference(conferenceId);
      const current = await this.client.fetchConference(conferenceId);

      return await factory(current);
   }
}
