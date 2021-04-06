import { useEffect, useState } from 'react';
import { ConsumerStatusInfo } from '../WebRtcConnection';
import useWebRtc from './useWebRtc';

export default function useConsumerStatusInfo(consumerId: string): ConsumerStatusInfo | undefined {
   const connection = useWebRtc();
   const [info, setInfo] = useState<ConsumerStatusInfo | undefined>();

   useEffect(() => {
      if (!connection) {
         setInfo(undefined);
         return;
      }

      const handleUpdateConsumerScore = ({ consumerId }: { consumerId: string }) => {
         setInfo(connection.getConsumerInfo(consumerId));
      };

      handleUpdateConsumerScore({ consumerId });
      connection.eventEmitter.on('onConsumerStatusInfoUpdated', handleUpdateConsumerScore);
      return () => {
         connection.eventEmitter.off('onConsumerStatusInfoUpdated', handleUpdateConsumerScore);
      };
   }, [connection]);

   return info;
}
