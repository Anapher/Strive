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
   constructor(private conn: RabbitMqConn) {}

   notifyConsumerClosed(connectionId: string, args: ConsumerArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'consumerClosed', payload: args });
   }

   notifyConsumerPaused(connectionId: string, args: ConsumerArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'consumerPaused', payload: args });
   }

   notifyConsumerResumed(connectionId: string, args: ConsumerArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'consumerResumed', payload: args });
   }

   notifyConsumerScore(connectionId: string, args: ConsumerScoreArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'consumerScore', payload: args });
   }

   notifyConsumerCreated(connectionId: string, args: ConsumerCreatedArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'newConsumer', payload: args });
   }

   notifyConsumerLayersChanged(connectionId: string, args: ConsumerLayersChanged): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'layersChanged', payload: args });
   }

   notifyProducerChanged(connectionId: string, args: ProducerChangedEventArgs): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'producerChanged', payload: args });
   }

   notifyProducerScore(connectionId: string, args: ProducerScoreInfo): Promise<void> {
      return this.notifyConnection({ connectionId, methodName: 'producerScore', payload: args });
   }

   updateStreams(streams: ConferenceParticipantStreamInfo, conferenceId: string): Promise<void> {
      const message = RabbitPub.buildMassTransitMessage(
         ['urn:message:Strive.Messaging.SFU.ReceiveContracts:StreamsUpdated'],
         { conferenceId, streams },
      );

      return this.internalPub(message);
   }

   private async internalPub(message: MassTransitMessage): Promise<void> {
      const blob = JSON.stringify(message);
      const channel = await this.conn.getChannel();

      channel.pub.sendToQueue('fromSfu', Buffer.from(blob), { contentType: 'application/vnd.masstransit+json' });
   }

   private static buildMassTransitMessage(messageType: string[], message: any): MassTransitMessage {
      return { messageType, message };
   }

   private async notifyConnection(payload: SendMessageToConnection): Promise<void> {
      const message = RabbitPub.buildMassTransitMessage(
         ['urn:message:Strive.Messaging.SFU.ReceiveContracts:SendMessageToConnection'],
         payload,
      );

      return this.internalPub(message);
   }
}

type MassTransitMessage = { message: any; messageType: string[] };
type SendMessageToConnection = { connectionId: string; methodName: string; payload: any };
