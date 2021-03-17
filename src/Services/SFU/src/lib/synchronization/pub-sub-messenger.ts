import { ConferenceInfo } from '../types';
import RabbitMqConn, { RabbitChannel } from '../../rabbitmq/rabbit-mq-conn';
import { SynchronizedConference } from './synchronized-conference';

export type SynchronizedConferenceFactory = (info: ConferenceInfo) => Promise<SynchronizedConference>;

export default class PubSubMessenger {
   private cachedChannel: RabbitChannel | undefined;

   constructor(private rabbit: RabbitMqConn) {}

   public async createSynchronizedConference(conferenceId: string): Promise<SynchronizedConferenceFactory> {
      const channel = await this.getChannel();

      const queue = await channel.sub.assertQueue('', { exclusive: true });
      await channel.sub.bindQueue(queue.queue, 'toSfu', conferenceId);

      return async (info: ConferenceInfo) => {
         const result = new SynchronizedConference(channel, queue.queue, info);
         await result.init();
         return result;
      };
   }

   private async getChannel(): Promise<RabbitChannel> {
      const channel = await this.rabbit.getChannel();
      if (this.cachedChannel === channel) return channel;

      await channel.sub.assertExchange('toSfu', 'direct', { durable: false });
      await channel.pub.assertExchange('fromSfu', 'direct', { durable: false });

      this.cachedChannel = channel;
      return channel;
   }
}
