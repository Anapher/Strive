import { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '..';
import { SoupManager } from './SoupManager';
import { useMedia, UseMediaState } from './useMedia';

export function useMicrophone(soupManager: SoupManager | null): UseMediaState {
   const audioGain = useSelector((state: RootState) => state.settings.obj.audioGain);
   const audioGainNode = useRef<GainNode | null>(null);

   useEffect(() => {
      if (audioGainNode.current) {
         audioGainNode.current.gain.value = audioGain;
      }
   }, [audioGain]);

   const getMic = async () => {
      if (!soupManager?.device.canProduce('audio')) {
         throw new Error('Cannot produce audio');
      }

      const audioContext = new AudioContext();
      const gainNode = audioContext.createGain();

      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

      const audioSource = audioContext.createMediaStreamSource(stream);
      const audioDestination = audioContext.createMediaStreamDestination();

      audioSource.connect(gainNode);
      gainNode.connect(audioDestination);
      gainNode.gain.value = audioGain;

      audioGainNode.current = gainNode;

      return audioDestination.stream.getAudioTracks()[0];
   };

   return useMedia('mic', getMic, () => soupManager?.sendTransport);
}
