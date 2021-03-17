import { ConferenceMessenger } from '../conference/conference-messenger';
import {
   ProducerChangedEventArgs,
   ProducerScoreInfo,
   ConferenceParticipantStreamInfo,
   ConsumerArgs,
   ConsumerCreatedArgs,
   ConsumerLayersChanged,
   ConsumerScoreArgs,
} from '../conference/pub-types';
import RabbitMqConn from '../../rabbitmq/rabbit-mq-conn';

export default class RabbitPub implements ConferenceMessenger {
   constructor(private conn: RabbitMqConn, private conferenceId: string) {}
   notifyConsumerClosed(connectionId: string, args: ConsumerArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }
   notifyConsumerPaused(connectionId: string, args: ConsumerArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }
   notifyConsumerResumed(connectionId: string, args: ConsumerArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }
   notifyConsumerScore(connectionId: string, args: ConsumerScoreArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }
   notifyConsumerCreated(connectionId: string, args: ConsumerCreatedArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }
   notifyConsumerLayersChanged(connectionId: string, args: ConsumerLayersChanged): Promise<void> {
      throw new Error('Method not implemented.');
   }

   notifyProducerChanged(connectionId: string, args: ProducerChangedEventArgs): Promise<void> {
      throw new Error('Method not implemented.');
   }

   notifyProducerScore(connectionId: string, args: ProducerScoreInfo): Promise<void> {
      throw new Error('Method not implemented.');
   }

   updateStreams(arg: ConferenceParticipantStreamInfo): Promise<void> {
      throw new Error('Method not implemented.');
   }

   private async internalPub(message: any): Promise<void> {
      const blob = JSON.stringify(message);
      const channel = await this.conn.getChannel();

      channel.pub.publish('fromSfu', this.conferenceId, Buffer.from(blob), { contentType: 'application/json' });
   }
}
