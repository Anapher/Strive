import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';
import { ProducerSource } from '../types';
import useWebRtc from './useWebRtc';

export default function useConsumer(participantId: string | undefined, kind: ProducerSource): Consumer | null {
   const [consumer, setConsumer] = useState<Consumer | null>(null);
   const connection = useWebRtc();

   useEffect(() => {
      let currentConsumer: string | undefined;
      if (connection && participantId) {
         const findConsumer = (consumers: IterableIterator<Consumer>) => {
            for (const c of consumers) {
               if (c.appData.participantId == participantId && c.appData.source === kind) {
                  return c;
               }
            }

            return null;
         };

         const initConsumer = (consumer: Consumer | null) => {
            currentConsumer = consumer?.id;
            setConsumer(consumer);
            if (consumer && consumer.paused) {
               connection.changeStream({ type: 'consumer', action: 'resume', id: consumer.id });
            }
         };

         const listener = () => {
            initConsumer(findConsumer(connection.getConsumers()));
         };

         const updateListener = ({ consumerId }: { consumerId: string }) => {
            if (currentConsumer === consumerId) {
               const consumer = findConsumer(connection.getConsumers());
               setConsumer(consumer);
               currentConsumer = consumer?.id;
            }
         };

         initConsumer(findConsumer(connection.getConsumers()));
         connection.eventEmitter.addListener('onConsumersChanged', listener);
         connection.eventEmitter.addListener('onConsumerUpdated', updateListener);
         return () => {
            connection.eventEmitter.removeListener('onConsumersChanged', listener);
            connection.eventEmitter.removeListener('onConsumerUpdated', updateListener);
         };
      } else {
         setConsumer(null);
      }
   }, [connection, participantId, kind]);

   return consumer;
}
