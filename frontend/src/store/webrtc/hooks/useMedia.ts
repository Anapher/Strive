import { Producer, ProducerOptions } from 'mediasoup-client/lib/types';
import { useRef, useState } from 'react';
import { ProducerSource } from '../types';
import useWebRtc from './useWebRtc';

export interface UseMediaControl {
   enable: () => Promise<void> | void;
   disable: () => Promise<void> | void;

   pause: () => Promise<void> | void;
   resume: () => Promise<void> | void;

   switchDevice: (deviceId?: string) => Promise<void> | void;
}

export type UseMediaStateInfo = {
   connected: boolean;
   enabled: boolean;
   paused: boolean;
   streamInfo?: CurrentStreamInfo;
};

export type UseMediaState = UseMediaControl & UseMediaStateInfo;

export type CurrentStreamInfo = {
   producerId: string;
   deviceId?: string;
};

export default function useMedia(
   source: ProducerSource,
   getMediaTrack: (deviceId?: string) => Promise<MediaStreamTrack>,
   options?: Partial<ProducerOptions>,
): UseMediaState {
   const producerRef = useRef<Producer | null>(null);
   const [enabled, setEnabled] = useState(false);
   const [paused, setPaused] = useState(false);
   const appliedDeviceId = useRef<string | undefined>(undefined);
   const [streamInfo, setStreamInfo] = useState<CurrentStreamInfo | undefined>(undefined);

   const connection = useWebRtc();

   const disable = async () => {
      if (!connection) return;
      if (!producerRef.current) return;

      const producerId = producerRef.current.id;

      producerRef.current.close();
      producerRef.current = null;

      setEnabled(false);
      setStreamInfo(undefined);

      await connection.changeStream({ id: producerId, type: 'producer', action: 'close' });
   };

   const enable = async () => {
      if (!connection) throw new Error('Not connected');
      if (producerRef.current) return;

      if (!connection.sendTransport) {
         throw new Error('Send transport must first be initialized');
      }

      const track = await getMediaTrack(appliedDeviceId.current);
      const producer = await connection.sendTransport.produce({ ...(options ?? {}), track, appData: { source } });
      producerRef.current = producer;

      producer.on('transportclose', () => {
         producerRef.current = null;
         setEnabled(false);
         setStreamInfo(undefined);
      });

      producer.on('trackended', () => {
         disable();
      });

      setEnabled(true);
      setStreamInfo({ producerId: producer.id, deviceId: track.getSettings().deviceId });
   };

   const pause = async () => {
      if (!connection) return;

      if (producerRef.current) {
         producerRef.current.pause();
         setPaused(true);

         await connection.changeStream({ id: producerRef.current.id, type: 'producer', action: 'pause' });
      }
   };

   const resume = async () => {
      if (!connection) return;
      if (producerRef.current) {
         producerRef.current.resume();
         setPaused(false);

         await connection.changeStream({ id: producerRef.current.id, type: 'producer', action: 'resume' });
      }
   };

   const switchDevice = async (deviceId?: string) => {
      if (deviceId === appliedDeviceId.current) return;
      appliedDeviceId.current = deviceId;

      if (!producerRef.current) return;

      const producer = producerRef.current;
      const track = await getMediaTrack(appliedDeviceId.current);

      await producer.replaceTrack({ track });
      setStreamInfo(streamInfo && { ...streamInfo, deviceId: track.getSettings().deviceId });
   };

   return { enable, disable, enabled, pause, resume, paused, switchDevice, connected: !!connection, streamInfo };
}
