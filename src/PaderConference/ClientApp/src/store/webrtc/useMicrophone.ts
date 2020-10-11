import { send } from 'src/store/conference-signal/actions';
import { Producer } from 'mediasoup-client/lib/types';
import { useRef, useState } from 'react';
import { useDispatch } from 'react-redux';
import { SoupManager } from './RtcManager';

type UseMicrophoneState = {
   enable: () => Promise<void>;
   disable: () => void;
   enabled: boolean;

   mute: () => void;
   unmute: () => void;
   muted: boolean;
};

export function useMicrophone(soup: SoupManager): UseMicrophoneState {
   const producerRef = useRef<Producer | null>(null);
   const [enabled, setEnabled] = useState(false);
   const [muted, setMuted] = useState(false);

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

      if (!soup.sendTransport) {
         throw new Error('Send transport must first be initialized');
      }

      if (!soup.device.canProduce('audio')) {
         throw new Error('Cannot produce audio');
      }

      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      const track = stream.getAudioTracks()[0];

      const producer = await soup.sendTransport.produce({ track });
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

   const mute = () => {
      if (producerRef.current) {
         producerRef.current.pause();
         setMuted(true);

         dispatch(send('MediaSoup_PauseProducer', { producerId: producerRef.current.id }));
      }
   };

   const unmute = () => {
      if (producerRef.current) {
         producerRef.current.resume();
         setMuted(false);

         dispatch(send('MediaSoup_ResumeProducer', { producerId: producerRef.current.id }));
      }
   };

   return { enable, disable, enabled, mute, unmute, muted };
}
