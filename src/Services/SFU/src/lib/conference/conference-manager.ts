import { RouterOptions, WebRtcTransportOptions } from 'mediasoup/lib/types';
import MediaSoupWorkers from '../../media-soup-workers';
import { ConferenceManagementClient } from '../synchronization/conference-management-client';
import { ConferenceRepository } from '../synchronization/conference-repository';
import RabbitMqConn from '../../rabbitmq/rabbit-mq-conn';
import { Conference } from './conference';
import conferenceFactory from './conference-factory';

export type ConferenceManagerOptions = {
   routerOptions: RouterOptions;
   webrtcOptions: WebRtcTransportOptions;
   maxIncomingBitrate?: number;
};

/**
 * Manages the local conferences this SFU handles
 */
export default class ConferenceManager {
   private conferences: Map<string, Conference> = new Map();
   private repository: ConferenceRepository;

   constructor(
      private rabbitConn: RabbitMqConn,
      private workers: MediaSoupWorkers,
      client: ConferenceManagementClient,
      private options: ConferenceManagerOptions,
   ) {
      this.repository = new ConferenceRepository(client, rabbitConn);
   }

   public async getConference(id: string): Promise<Conference> {
      let localConference = this.conferences.get(id);
      if (!localConference) {
         localConference = await conferenceFactory(id, this.workers, this.repository, this.rabbitConn, this.options);

         this.conferences.set(id, localConference);
         await this.registerConferenceEvents(id);
      }

      return localConference;
   }

   public hasConference(id: string): boolean {
      return this.conferences.has(id);
   }

   private async registerConferenceEvents(id: string): Promise<void> {
      await this.repository.addMessageHandler(id, (message) => {
         // todo
      });
   }
}
