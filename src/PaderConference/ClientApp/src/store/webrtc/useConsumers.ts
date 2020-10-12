import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';
import { SoupManager } from './SoupManager';

export default function useConsumers(soup?: SoupManager): Consumer[] {
   const [consumers, setConsumers] = useState<Consumer[]>([]);

   useEffect(() => {
      if (soup) {
         const listener = () => {
            setConsumers(Array.from(soup.getConsumers()));
         };

         setConsumers(Array.from(soup.getConsumers()));

         soup.eventEmitter.addListener('onConsumersChanged', listener);
         return () => {
            soup.eventEmitter.removeListener('onConsumersChanged', listener);
         };
      }
   }, [soup]);

   return consumers;
}
