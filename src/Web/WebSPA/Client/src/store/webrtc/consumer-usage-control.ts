import debug from 'debug';
import { Consumer } from 'mediasoup-client/lib/types';
import { WebRtcConnection } from './WebRtcConnection';

const PAUSE_CONSUMER_TIMEOUT = 1000;

const log = debug('webrtc:ConsumerControl');

export type UnregisterConsumerCallback = () => void;

/**
 * Manage the consumer usage, if a consumer is activly used its resumed and if its not used its paused
 * This class keeps a counter to all references of a consumer and manages the pause/resume actions
 */
class ConsumerUsageControl {
   private consumerUsage = new Map<string, number>();
   private deletionTimeouts = new Map<string, NodeJS.Timeout>();

   constructor(private connection: WebRtcConnection) {
      connection.consumerManager.on('consumerRemoved', (id) => {
         this.consumerUsage.delete(id);
         this.removeTimeout(id);
      });
   }

   /**
    * State that a consumer is used at a specific place. This method takes care of keeping the state of
    * the consumer not paused until the callback (of the return value) is called
    * @param id the consumer id
    */
   public useConsumer(id: string): [Consumer, UnregisterConsumerCallback] | undefined {
      log('Use consumer %s', id);

      const consumer = this.connection.consumerManager.getConsumer(id);
      if (!consumer) {
         log('Consumer was not found');
         return undefined;
      }

      let currentUsage = this.consumerUsage.get(id);
      if (currentUsage === undefined) {
         log('Consumer was found, its a new consumer');
         currentUsage = 1;
      } else {
         log('Consumer was found, its already used at %d points', currentUsage);
         currentUsage++;
      }

      this.consumerUsage.set(id, currentUsage);

      this.removeTimeout(id);

      if (currentUsage === 1) {
         log('Consumer was not used before, resume...');
         this.connection.changeStream({ id: consumer.id, type: 'consumer', action: 'resume' });
      }

      const unregisterCallback = () => this.unregisterConsumer(id);
      return [consumer, unregisterCallback];
   }

   private unregisterConsumer(id: string): void {
      log('Unregister consumer %s', id);

      let currentUsage = this.consumerUsage.get(id);
      if (currentUsage === undefined) {
         log('Consumer was not used...');
         return;
      }

      log('Consumer was used by %d points', currentUsage);

      currentUsage--;

      if (currentUsage > 0) {
         log('Consumer is still used by other points, decrease counter and return');

         this.consumerUsage.set(id, currentUsage);
         return;
      }

      this.consumerUsage.delete(id);
      log('Consumer is not used anymore, set timeout for deletion');

      // unregister the consumer using a timeout to prevent unnecessary pause/resume sequences if a consumer is just moved in the UI
      const timeout = setTimeout(() => {
         this.deletionTimeouts.delete(id);

         log('Consumer %s pause timeout passed', id);

         const consumer = this.connection.consumerManager.getConsumer(id);
         if (!consumer) {
            log('Consumer was not found');
            return;
         }

         if (this.consumerUsage.get(id)) return;

         log('Send pause request');
         this.connection.changeStream({ id: consumer.id, type: 'consumer', action: 'pause' });
      }, PAUSE_CONSUMER_TIMEOUT);

      this.deletionTimeouts.set(id, timeout);
   }

   private removeTimeout(id: string) {
      const timeout = this.deletionTimeouts.get(id);
      if (timeout) {
         clearTimeout(timeout);
         this.deletionTimeouts.delete(id);
      }
   }
}

export default ConsumerUsageControl;
