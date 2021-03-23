import { RouterOptions, WebRtcTransportOptions } from 'mediasoup/lib/types';
import MediaSoupWorkers from '../../media-soup-workers';
import { ConferenceManagementClient } from '../synchronization/conference-management-client';
import { ConferenceRepository } from '../synchronization/conference-repository';
import RabbitMqConn from '../../rabbitmq/rabbit-mq-conn';
import { Conference } from './conference';
import conferenceFactory from './conference-factory';
import _ from 'lodash';

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
         await this.registerConferenceEvents(id, localConference);
      }

      return localConference;
   }

   public hasConference(id: string): boolean {
      return this.conferences.has(id);
   }

   private async registerConferenceEvents(id: string, conference: Conference): Promise<void> {
      await this.repository.addMessageHandler(id, async (message) => {
         switch (message.type) {
            case 'conferenceInfoUpdated':
               const unfreezeCallback = conference.streamInfoRepo.freeze();
               try {
                  for (const participantId of message.update.removedParticipants) {
                     await conference.removeParticipant(participantId);
                  }

                  const updatedParticipants = _.uniq(
                     Array.from(message.update.participantToRoom.keys()).concat(
                        Array.from(message.update.participantPermissions.keys()),
                     ),
                  );

                  for (const participantId of updatedParticipants) {
                     await conference.updateParticipant(participantId);
                  }
               } finally {
                  await unfreezeCallback();
               }
               break;
            case 'producerChanged':
               await conference.changeProducerSource(message.dto, message.dto.participantId);
               break;
            default:
               break;
         }
      });
   }
}
