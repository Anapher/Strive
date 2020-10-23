import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';
import { SoupManager } from './SoupManager';

export default function useConsumer(
   soup: SoupManager | null,
   participantId: string | undefined,
   kind: string,
): Consumer | null {
   const [consumer, setConsumer] = useState<Consumer | null>(null);

   useEffect(() => {
      if (soup && participantId) {
         const findConsumer = (consumers: IterableIterator<Consumer>) => {
            for (const c of consumers) {
               if (c.appData.participantId == participantId && c.kind === kind) {
                  return c;
               }
            }

            return null;
         };

         const listener = () => {
            setConsumer(findConsumer(soup.getConsumers()));
         };

         setConsumer(findConsumer(soup.getConsumers()));
         soup.eventEmitter.addListener('onConsumersChanged', listener);
         return () => {
            soup.eventEmitter.removeListener('onConsumersChanged', listener);
         };
      }
   }, [soup, participantId, kind]);

   return consumer;
}
