import RabbitMqConn, { RabbitChannel } from '../../rabbitmq/rabbit-mq-conn';
import { SynchronizedConference } from './synchronized-conference';

export default class PubSubMessenger {
   private cachedChannel: RabbitChannel | undefined;

   constructor(private rabbit: RabbitMqConn) {}

   public async createSynchronizedConference(conferenceId: string): Promise<SynchronizedConference> {
      const channel = await this.getChannel();

      const queue = await channel.sub.assertQueue('', { exclusive: true });
      await channel.sub.bindQueue(queue.queue, 'toSfu', conferenceId);

      const syncConference = new SynchronizedConference(channel, queue.queue);
      await syncConference.start();

      return syncConference;
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
