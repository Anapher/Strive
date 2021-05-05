import { Consumer } from 'mediasoup-client/lib/types';
import { TypedEmitter } from 'tiny-typed-emitter';
import { ConsumerStatusInfo } from './WebRtcConnection';

interface ConsumerManagerEvents {
   consumerAdded: (id: string) => void;
   consumerRemoved: (id: string) => void;
   consumerUpdated: (id: string) => void;

   consumerInfoUpdated: (id: string) => void;
}

/**
 * Manage the consumers and information about the consumers. Acts like a repository and takes care
 * of emitting events
 */
class ConsumerManager extends TypedEmitter<ConsumerManagerEvents> {
   private consumers = new Map<string, Consumer>();
   private consumerInfo = new Map<string, ConsumerStatusInfo>();

   public addConsumer(consumer: Consumer) {
      this.consumers.set(consumer.id, consumer);
      consumer.on('transportclose', () => this.removeConsumer(consumer));

      this.emit('consumerAdded', consumer.id);
   }

   public removeConsumer(consumer: Consumer) {
      this.consumers.delete(consumer.id);
      this.consumerInfo.delete(consumer.id);

      this.emit('consumerRemoved', consumer.id);
   }

   public pausedConsumer(id: string) {
      this.consumerInfo.delete(id);
      this.onConsumerUpdated(id);
   }

   public resumedConsumer(id: string) {
      this.onConsumerUpdated(id);
   }

   public getConsumers(): IterableIterator<Consumer> {
      return this.consumers.values();
   }

   public getConsumer(id: string): Consumer | undefined {
      return this.consumers.get(id);
   }

   public getConsumerInfo(consumerId: string): ConsumerStatusInfo | undefined {
      return this.consumerInfo.get(consumerId);
   }

   public updateConsumerInfo(consumerId: string, update: Partial<ConsumerStatusInfo>) {
      if (!this.consumers.has(consumerId)) return;

      this.consumerInfo.set(consumerId, {
         ...this.consumerInfo.get(consumerId),
         ...update,
      });

      this.emit('consumerInfoUpdated', consumerId);
   }

   private onConsumerUpdated(id: string) {
      this.emit('consumerUpdated', id);
   }
}

export default ConsumerManager;
