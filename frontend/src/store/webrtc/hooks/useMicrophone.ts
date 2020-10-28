import { useEffect, useRef } from 'react';
import { useMedia, UseMediaState } from './useMedia';

export function useMicrophone(gain?: number): UseMediaState {
   const audioGainNode = useRef<GainNode | null>(null);

   useEffect(() => {
      if (audioGainNode.current) {
         audioGainNode.current.gain.value = gain ?? 1;
      }
   }, [gain]);

   const getMic = async () => {
      const audioContext = new AudioContext();
      const gainNode = audioContext.createGain();

      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

      const audioSource = audioContext.createMediaStreamSource(stream);
      const audioDestination = audioContext.createMediaStreamDestination();

      audioSource.connect(gainNode);
      gainNode.connect(audioDestination);
      gainNode.gain.value = gain ?? 1;

      audioGainNode.current = gainNode;

      return audioDestination.stream.getAudioTracks()[0];
   };

   return useMedia('mic', getMic);
}
