import { Consumer } from 'mediasoup-client/lib/types';
import { useEffect, useState } from 'react';

export default function useConsumerMediaStream(consumer: Consumer | undefined | null) {
   const [stream, setStream] = useState<MediaStream | undefined>();

   useEffect(() => {
      if (consumer) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         setStream(stream);

         return () => {
            setStream(undefined);
         };
      }
   }, [consumer]);

   return stream;
}
