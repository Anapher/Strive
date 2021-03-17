import { EventEmitter } from 'events';
import { ConferenceInfo } from '../types';
import { RabbitChannel } from '../../rabbitmq/rabbit-mq-conn';

/**
 * Holds a conference info that is synchronized using rabbit mq
 */
export class SynchronizedConference extends EventEmitter {
   constructor(private channel: RabbitChannel, private queue: string, private info: ConferenceInfo) {
      super();
   }

   /**
    * Activate the synchronization through rabbit mq
    */
   public async init(): Promise<void> {
      await this.channel.sub.consume(
         this.queue,
         (message) => {
            if (message) {
               this.processMessage(message.content.toString());
            }
         },
         { noAck: true },
      );
   }

   /**
    * Get a snapshot of the current conference info
    */
   public get conferenceInfo(): ConferenceInfo {
      return this.info;
   }

   private processMessage(message: string) {
      console.log('message from rabbit', message);

      // todo
   }

   public async close(): Promise<void> {
      await this.channel.sub.deleteQueue(this.queue);
   }
}
