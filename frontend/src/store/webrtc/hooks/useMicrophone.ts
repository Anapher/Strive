import { useEffect, useRef } from 'react';
import useMedia, { UseMediaState } from './useMedia';

export default function useMicrophone(gain?: number): UseMediaState {
   const audioGainNode = useRef<GainNode | null>(null);

   useEffect(() => {
      if (audioGainNode.current) {
         audioGainNode.current.gain.value = gain ?? 1;
      }
   }, [gain]);

   const getMic = async (deviceId?: string) => {
      const audioContext = new AudioContext();
      const gainNode = audioContext.createGain();

      const stream = await navigator.mediaDevices.getUserMedia({ audio: { deviceId } });

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
