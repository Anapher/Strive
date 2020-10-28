import { Producer, ProducerOptions } from 'mediasoup-client/lib/types';
import { useRef, useState } from 'react';
import { ProducerSource } from 'src/features/media/types';
import useWebRtc from './useWebRtc';

export type UseMediaState = {
   connected: boolean;

   enable: () => Promise<void>;
   disable: () => void;
   enabled: boolean;

   pause: () => void;
   resume: () => void;
   paused: boolean;
};

export function useMedia(
   source: ProducerSource,
   getMediaTrack: () => Promise<MediaStreamTrack>,
   options?: Partial<ProducerOptions>,
): UseMediaState {
   const producerRef = useRef<Producer | null>(null);
   const [enabled, setEnabled] = useState(false);
   const [paused, setPaused] = useState(false);

   const connection = useWebRtc();

   const disable = () => {
      if (!connection) return;
      if (!producerRef.current) return;

      producerRef.current.close();
      connection.changeStream({ id: producerRef.current.id, type: 'producer', action: 'close' });
      producerRef.current = null;

      setEnabled(false);
   };

   const enable = async () => {
      if (!connection) throw new Error('Not connected');
      if (producerRef.current) return;

      if (!connection.sendTransport) {
         throw new Error('Send transport must first be initialized');
      }

      const track = await getMediaTrack();
      const producer = await connection.sendTransport.produce({ ...(options ?? {}), track, appData: { source } });
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
      if (!connection) return;

      if (producerRef.current) {
         producerRef.current.pause();
         setPaused(true);

         connection.changeStream({ id: producerRef.current.id, type: 'producer', action: 'pause' });
      }
   };

   const resume = () => {
      if (!connection) return;
      if (producerRef.current) {
         producerRef.current.resume();
         setPaused(false);

         connection.changeStream({ id: producerRef.current.id, type: 'producer', action: 'resume' });
      }
   };

   return { enable, disable, enabled, pause, resume, paused, connected: !!connection };
}
