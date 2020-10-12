import { send } from 'src/store/conference-signal/actions';
import { Producer, Transport } from 'mediasoup-client/lib/types';
import { useRef, useState } from 'react';
import { useDispatch } from 'react-redux';

export type UseMediaState = {
   enable: () => Promise<void>;
   disable: () => void;
   enabled: boolean;

   pause: () => void;
   resume: () => void;
   paused: boolean;
};

export function useMedia(
   getMediaTrack: () => Promise<MediaStreamTrack>,
   getSendTranspot: () => Transport | null | undefined,
): UseMediaState {
   const producerRef = useRef<Producer | null>(null);
   const [enabled, setEnabled] = useState(false);
   const [paused, setPaused] = useState(false);

   const dispatch = useDispatch();

   const disable = () => {
      if (!producerRef.current) return;

      producerRef.current.close();
      dispatch(send('MediaSoup_CloseProducer', { producerId: producerRef.current.id }));
      producerRef.current = null;

      setEnabled(false);
   };

   const enable = async () => {
      if (producerRef.current) return;

      const sendTransport = getSendTranspot();
      if (!sendTransport) {
         throw new Error('Send transport must first be initialized');
      }

      const track = await getMediaTrack();
      const producer = await sendTransport.produce({ track });
      producerRef.current = producer;

      producer.on('transportclose', () => {
         producerRef.current = null;
         setEnabled(false);
      });

      producer.on('trackended', () => {
         disable();
      });

      setEnabled(true);
   };

   const pause = () => {
      if (producerRef.current) {
         producerRef.current.pause();
         setPaused(true);

         dispatch(send('MediaSoup_PauseProducer', { producerId: producerRef.current.id }));
      }
   };

   const resume = () => {
      if (producerRef.current) {
         producerRef.current.resume();
         setPaused(false);

         dispatch(send('MediaSoup_ResumeProducer', { producerId: producerRef.current.id }));
      }
   };

   return { enable, disable, enabled, pause, resume, paused };
}
