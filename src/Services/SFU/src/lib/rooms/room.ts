import { Producer, Router } from 'mediasoup/lib/types';
import Logger from '../../utils/logger';
import { ConferenceMessenger } from '../conference/conference-messenger';
import Connection from '../connection';
import { MediasoupMixer } from '../media-soup/mediasoup-mixer';
import { Participant } from '../participant';
import { ParticipantPermissions } from '../participant-permissions';
import { MEDIA_CAN_SHARE_AUDIO, MEDIA_CAN_SHARE_SCREEN, MEDIA_CAN_SHARE_WEBCAM, Permission } from '../permissions';
import { ConferenceRepository } from '../synchronization/conference-repository';
import { ProducerSource, producerSources } from '../types';

type ParticipantStatus = {
   activeProducers: { [key in ProducerSource]?: Producer | undefined };
   receivingConn: Connection | undefined;
};

const producerPermission: { [key in ProducerSource]?: Permission<boolean> } = {
   mic: MEDIA_CAN_SHARE_AUDIO,
   webcam: MEDIA_CAN_SHARE_WEBCAM,
   screen: MEDIA_CAN_SHARE_SCREEN,
   /** no permissions required to loopback device */
};

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
      private supportedProducerSources: ProducerSource[],
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
         receivingConn: undefined,
      };

      this.participantStatus.set(participant.participantId, status);
      this.participants.set(participant.participantId, participant);

      await this.updateParticipant(participant);
   }

   async updateParticipant(participant: Participant): Promise<void> {
      const status = this.participantStatus.get(participant.participantId);
      if (status) {
         logger.info('updateParticipant() | participantId: %s | roomId: %s', participant.participantId, this.id);

         const receiveConnectionChanged = status.receivingConn !== participant.receiveConnection;
         if (receiveConnectionChanged) {
            if (status.receivingConn) {
               await this.mixer.removeReceiveTransport(status.receivingConn.connectionId);
            }

            if (participant.receiveConnection) {
               await this.mixer.addReceiveTransport(participant.receiveConnection);
               status.receivingConn = participant.receiveConnection;
            }
         }

         const conferenceInfo = await this.repo.getConference(this.conferenceId);
         const permissions = new ParticipantPermissions(participant.participantId, conferenceInfo);

         for (const source of producerSources) {
            if (!this.supportedProducerSources.includes(source)) continue;

            const activeProducer = status.activeProducers[source];
            const newProducer = Room.getProducer(participant, source, permissions);

            const producerChanged = newProducer?.producer.id !== activeProducer?.id;
            if (producerChanged) {
               if (activeProducer) {
                  // if a producer was active and changed
                  this.mixer.removeProducer(activeProducer.id);
                  status.activeProducers[source] = undefined;
               }

               if (newProducer) {
                  await this.mixer.addProducer({
                     participantId: participant.participantId,
                     producer: newProducer.producer,
                  });
                  status.activeProducers[source] = newProducer.producer;
               }
            }
         }
      }
   }

   async leave(participant: Participant): Promise<void> {
      logger.info('leave() | participantId: %s | roomId: %s', participant.participantId, this.id);

      const status = this.participantStatus.get(participant.participantId);
      if (status) {
         if (status.receivingConn) {
            await this.mixer.removeReceiveTransport(status.receivingConn.connectionId);
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

   private static getProducer(participant: Participant, source: ProducerSource, permissions: ParticipantPermissions) {
      if (!participant.producers[source]) return;

      const requiredPermission = producerPermission[source];

      if (!requiredPermission) return participant.producers[source];
      if (permissions.get(requiredPermission)) return participant.producers[source];

      logger.debug(
         'getProducer() | participantId: %s | Permission denied for producer %s',
         participant.participantId,
         source,
      );

      return undefined;
   }
}
