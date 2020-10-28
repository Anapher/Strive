import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';
import useWebRtc from './useWebRtc';

export default function useConsumer(participantId: string | undefined, kind: string): Consumer | null {
   const [consumer, setConsumer] = useState<Consumer | null>(null);
   const connection = useWebRtc();

   useEffect(() => {
      if (connection && participantId) {
         const findConsumer = (consumers: IterableIterator<Consumer>) => {
            for (const c of consumers) {
               if (c.appData.participantId == participantId && c.kind === kind) {
                  return c;
               }
            }

            return null;
         };

         const initConsumer = (consumer: Consumer | null) => {
            setConsumer(consumer);
            if (consumer && consumer.paused) {
               connection.changeStream({ type: 'consumer', action: 'resume', id: consumer.id });
            }
         };

         const listener = () => {
            initConsumer(findConsumer(connection.getConsumers()));
         };

         initConsumer(findConsumer(connection.getConsumers()));
         connection.eventEmitter.addListener('onConsumersChanged', listener);
         return () => {
            connection.eventEmitter.removeListener('onConsumersChanged', listener);
         };
      }
   }, [connection, participantId, kind]);

   return consumer;
}
