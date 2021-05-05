import { useEffect, useState } from 'react';
import { ConsumerStatusInfo } from '../WebRtcConnection';
import useWebRtc from './useWebRtc';

export default function useConsumerStatusInfo(consumerId: string): ConsumerStatusInfo | null {
   const connection = useWebRtc();
   const [info, setInfo] = useState<ConsumerStatusInfo | null>(null);

   useEffect(() => {
      if (!connection) {
         setInfo(null);
         return;
      }

      const handleUpdateConsumerScore = (id: string) => {
         if (id === consumerId) {
            setInfo(connection.consumerManager.getConsumerInfo(consumerId) ?? null);
         }
      };

      handleUpdateConsumerScore(consumerId);

      connection.consumerManager.on('consumerInfoUpdated', handleUpdateConsumerScore);
      return () => {
         connection.consumerManager.off('consumerInfoUpdated', handleUpdateConsumerScore);
      };
   }, [connection, consumerId]);

   return info;
}
