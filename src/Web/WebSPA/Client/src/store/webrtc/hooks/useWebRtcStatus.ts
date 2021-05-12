import { useContext, useEffect, useState } from 'react';
import { WebRtcContext } from '../WebRtcContext';
import { WebRtcStatus } from '../WebRtcManager';

const useWebRtcStatus = () => {
   const context = useContext(WebRtcContext);
   const [status, setStatus] = useState<WebRtcStatus>(context.status);

   useEffect(() => {
      const onUpdate = () => setStatus(context.status);

      context.on('statuschanged', onUpdate);
      return () => {
         context.off('statuschanged', onUpdate);
      };
   }, [context]);

   return status;
};

export default useWebRtcStatus;
