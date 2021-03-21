import { EventEmitter } from 'events';
import { ConferenceInfo, ConferenceInfoUpdate, ConferenceInfoUpdateDto } from '../types';
import { RabbitChannel } from '../../rabbitmq/rabbit-mq-conn';
import { objectToMap } from '../../utils/map-utils';
import { ReceivedSfuMessage } from './message-types';

/**
 * Holds a conference info that is synchronized using rabbit mq
 */
export class SynchronizedConference extends EventEmitter {
   private info: ConferenceInfo | undefined;
   private cachedUpdates = new Array<ConferenceInfoUpdate>();

   constructor(private channel: RabbitChannel, private queue: string) {
      super();
   }

   /**
    * Activate the synchronization with rabbit mq
    */
   public async start(): Promise<void> {
      await this.channel.sub.consume(
         this.queue,
         (message) => {
            if (message) {
               const messageDeserialized = JSON.parse(message.content.toString());

               const sfuMessage = messageDeserialized.message as ReceivedSfuMessage;
               this.processMessage(sfuMessage);
            }
         },
         { noAck: true },
      );
   }

   public initialize(value: ConferenceInfo): void {
      if (this.info) throw new Error('Synchronized conference can only be initialized once');

      const updated = this.cachedUpdates.reduce((previous, update) => applyUpdate(previous, update), value);
      this.info = updated;

      this.cachedUpdates = [];
   }

   /**
    * Get a snapshot of the current conference info
    */
   public get conferenceInfo(): ConferenceInfo {
      if (!this.info) throw new Error('The synchronized conference must first be initalized.');
      return this.info;
   }

   private processMessage(message: ReceivedSfuMessage) {
      switch (message.type) {
         case 'Update':
            const update = message.payload;
            const fixed: ConferenceInfoUpdate = {
               participantPermissions: objectToMap(update.participantPermissions),
               participantToRoom: objectToMap(update.participantToRoom),
               removedParticipants: update.removedParticipants,
            };

            this.processUpdate(fixed);
            break;
         case 'ChangeProducer':
            this.emit('changeProducer');
            break;
         default:
            break;
      }
   }

   private processUpdate(update: ConferenceInfoUpdate) {
      if (!this.info) {
         this.cachedUpdates.push(update);
         return;
      }

      this.info = applyUpdate(this.info, update);
      this.emit('message', update);
   }

   public async close(): Promise<void> {
      await this.channel.sub.deleteQueue(this.queue);
   }
}

export function applyUpdate(conference: ConferenceInfo, update: ConferenceInfoUpdate): ConferenceInfo {
   const newPermissions = new Map(conference.participantPermissions);
   const newParticipants = new Map(conference.participantToRoom);

   for (const [participantId, permissions] of update.participantPermissions.entries()) {
      newPermissions.set(participantId, permissions);
   }

   for (const [participantId, roomId] of update.participantToRoom) {
      newParticipants.set(participantId, roomId);
   }

   for (const participantId of update.removedParticipants) {
      newPermissions.delete(participantId);
      newParticipants.delete(participantId);
   }

   return { participantPermissions: newPermissions, participantToRoom: newParticipants };
}
