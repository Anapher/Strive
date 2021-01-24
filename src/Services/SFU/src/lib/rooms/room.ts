import { ProducerLink } from './../types';
import { Producer, Router } from 'mediasoup/lib/types';
import { MEDIA_CAN_SHARE_AUDIO, MEDIA_CAN_SHARE_SCREEN, MEDIA_CAN_SHARE_WEBCAM, Permission } from '../permissions';
import Logger from '../../utils/logger';
import { ConferenceMessenger } from '../conference/conference-messenger';
import Connection from '../connection';
import { MediasoupMixer } from '../media-soup/mediasoup-mixer';
import { Participant } from '../participant';
import { ProducerSource, producerSources } from '../types';
import { ParticipantPermissions } from '../participant-permissions';
import { ConferenceRepository } from '../synchronization/conference-repository';

type ProducerPermission = {
   permission?: Permission<boolean>;
   source: ProducerSource;
};

type ParticipantStatus = {
   activeProducers: { [key in ProducerSource]?: Producer | undefined };
   receivingConns: Connection[];
};

const producerPermissions: ProducerPermission[] = [
   { permission: MEDIA_CAN_SHARE_AUDIO, source: 'mic' },
   { permission: MEDIA_CAN_SHARE_SCREEN, source: 'screen' },
   { permission: MEDIA_CAN_SHARE_WEBCAM, source: 'webcam' },

   /** no permissions required to loopback device */
   { permission: undefined, source: 'loopback-mic' },
   { permission: undefined, source: 'loopback-webcam' },
   { permission: undefined, source: 'loopback-screen' },
];

const logger = new Logger('Room');

/** each participant is in excactly one room. The room determines to which participants the media is sent */
export default class Room {
   private mixer: MediasoupMixer;

   constructor(
      public id: string,
      signal: ConferenceMessenger,
      router: Router,
      private repo: ConferenceRepository,
      private conferenceId: string,
      private producerSources: ProducerSource[],
   ) {
      this.mixer = new MediasoupMixer(router, signal);
   }

   public participants = new Map<string, Participant>();
   private participantStatus = new Map<string, ParticipantStatus>();

   getIsParticipantJoined(participantId: string): boolean {
      return this.participants.has(participantId);
   }

   async join(participant: Participant): Promise<void> {
      logger.info('join() | participantId: %s | roomId: %s', participant.participantId, this.id);

      const status: ParticipantStatus = {
         activeProducers: {},
         receivingConns: [],
      };

      const receiveConn = participant.getReceiveConnection();
      if (receiveConn) {
         this.mixer.addReceiveTransport(receiveConn);
         status.receivingConns.push(receiveConn);
      } else {
         logger.debug('No receive transport for %s', participant.participantId);
      }

      this.participantStatus.set(participant.participantId, status);
      this.participants.set(participant.participantId, participant);

      await this.updateParticipant(participant);
   }

   async updateParticipant(participant: Participant): Promise<void> {
      const status = this.participantStatus.get(participant.participantId);
      if (status) {
         logger.info('updateParticipant() | participantId: %s | roomId: %s', participant.participantId, this.id);

         const receiveConn = participant.getReceiveConnection();
         if (receiveConn && !status.receivingConns.includes(receiveConn)) {
            await this.mixer.addReceiveTransport(receiveConn);
            status.receivingConns.push(receiveConn);
         }

         const conferenceInfo = await this.repo.getConference(this.conferenceId);
         const permissions = new ParticipantPermissions(participant.participantId, conferenceInfo);

         for (const { permission, source } of producerPermissions) {
            if (!this.producerSources.includes(source)) continue;

            const activeProducer = status.activeProducers[source];
            const currentProducer = participant.producers[source];

            let newActiveProducer: ProducerLink | undefined;
            if (currentProducer) {
               if (!permission || permissions.get(permission)) {
                  newActiveProducer = currentProducer;
               } else {
                  newActiveProducer = undefined;
               }
            }

            if (!this.producerSources.includes(source)) continue;

            if (activeProducer && newActiveProducer?.producer.id !== activeProducer.id) {
               // if a producer was active and changed
               this.mixer.removeProducer(activeProducer.id);
               status.activeProducers[source] = undefined;
            }

            if (newActiveProducer) {
               await this.mixer.addProducer({
                  participantId: participant.participantId,
                  producer: newActiveProducer.producer,
               });
               status.activeProducers[source] = newActiveProducer.producer;
            }
         }
      }
   }

   async leave(participant: Participant): Promise<void> {
      logger.info('leave() | participantId: %s | roomId: %s', participant.participantId, this.id);

      const status = this.participantStatus.get(participant.participantId);
      if (status) {
         for (const receivingConn of status.receivingConns) {
            await this.mixer.removeReceiveTransport(receivingConn.connectionId);
         }

         for (const source of producerSources) {
            const producer = status.activeProducers[source];
            if (producer) {
               this.mixer.removeProducer(producer.id);
            }
         }
      }

      this.participants.delete(participant.participantId);
      this.participantStatus.delete(participant.participantId);
   }
}
