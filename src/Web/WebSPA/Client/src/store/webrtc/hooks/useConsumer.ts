import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';
import { UnregisterConsumerCallback } from '../consumer-usage-control';
import { ProducerSource } from '../types';
import useWebRtc from './useWebRtc';

const findConsumer = (consumers: IterableIterator<Consumer>, participantId: string, kind: ProducerSource) => {
   for (const c of consumers) {
      if (c.appData.participantId == participantId && c.appData.source === kind) {
         return c;
      }
   }

   return null;
};

/**
 * Use a consumer
 * @param participantId the participant id
 * @param kind the consumer kind
 * @returns return undefined if not initialized, null if consumer was not found and the consumer if found
 */
export default function useConsumer(
   participantId: string | undefined,
   kind: ProducerSource,
): Consumer | null | undefined {
   const [consumer, setConsumer] = useState<Consumer | null | undefined>(undefined);
   const connection = useWebRtc();

   useEffect(() => {
      let currentConsumer: string | undefined;
      let unregister: UnregisterConsumerCallback | undefined;

      if (connection && participantId) {
         const updateConsumer = () => {
            const consumerId = findConsumer(connection.consumerManager.getConsumers(), participantId, kind)?.id;
            let consumerUse: [Consumer, UnregisterConsumerCallback] | undefined;

            if (consumerId) {
               consumerUse = connection.consumerUsageControl.useConsumer(consumerId);
            }

            unregister?.();
            unregister = undefined;

            if (!consumerUse) {
               currentConsumer = undefined;
               setConsumer(null);
            } else {
               const [newConsumer, dispose] = consumerUse;
               currentConsumer = newConsumer.id;
               setConsumer(newConsumer);
               unregister = dispose;
            }
         };

         const onConsumerAdded = () => {
            if (!currentConsumer) {
               updateConsumer();
            }
         };

         const onConsumerRemoved = (id: string) => {
            if (id === currentConsumer) {
               updateConsumer();
            }
         };

         updateConsumer();

         connection.consumerManager.on('consumerAdded', onConsumerAdded);
         connection.consumerManager.on('consumerRemoved', onConsumerRemoved);
         return () => {
            connection.consumerManager.off('consumerAdded', onConsumerAdded);
            connection.consumerManager.off('consumerRemoved', onConsumerRemoved);

            unregister?.();
         };
      } else {
         setConsumer(null);
      }
   }, [connection, participantId, kind]);

   return consumer;
}
