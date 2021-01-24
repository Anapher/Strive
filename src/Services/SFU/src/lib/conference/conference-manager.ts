import { RouterOptions, WebRtcTransportOptions } from 'mediasoup/lib/types';
import MediaSoupWorkers from '../../media-soup-workers';
import { ConferenceManagementClient } from '../synchronization/conference-management-client';
import { ConferenceRepository } from '../synchronization/conference-repository';
import RabbitMqConn from '../synchronization/rabbit-mq-conn';
import { Conference } from './conference';
import conferenceFactory from './conference-factory';

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
      private routerOptions: RouterOptions,
      private webrtcOptions: WebRtcTransportOptions,
      private maxIncomingBitrate?: number,
   ) {
      this.repository = new ConferenceRepository(client, rabbitConn);

      rabbitConn.on('error', async () => {
         await rabbitConn.getChannel(); // reconnect
         for (const conference of this.conferences.keys()) {
            await this.registerConferenceEvents(conference);
         }
      });
   }

   public async getConference(id: string): Promise<Conference | undefined> {
      let localConference = this.conferences.get(id);
      if (!localConference) {
         localConference = await conferenceFactory(
            id,
            this.workers,
            this.repository,
            this.rabbitConn,
            this.routerOptions,
            this.webrtcOptions,
            this.maxIncomingBitrate,
         );

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
