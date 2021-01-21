import { useContext, useEffect, useState } from 'react';
import { WebRtcContext } from '../WebRtcContext';

const useWebRtc = () => {
   const context = useContext(WebRtcContext);

   const [webRtc, setWebRtc] = useState(context.current);

   useEffect(() => {
      const onUpdate = () => setWebRtc(context.current);

      context.on('update', onUpdate);
      return () => {
         context.off('update', onUpdate);
      };
   }, [context]);

   return webRtc;
};

export default useWebRtc;
