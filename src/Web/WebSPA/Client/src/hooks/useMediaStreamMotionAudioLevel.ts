import { useMotionValue, useSpring, useTransform } from 'framer-motion';
import hark from 'hark';
import { useEffect } from 'react';

export default function useMediaStreamMotionAudioLevel(stream: MediaStream | undefined | null) {
   const audioLevel = useMotionValue(0);

   useEffect(() => {
      if (stream) {
         const analyser = hark(stream, { play: false });
         analyser.on('volume_change', (dBs) => {
            // The exact formula to convert from dBs (-100..0) to linear (0..1) is:
            //   Math.pow(10, dBs / 20)
            // However it does not produce a visually useful output, so let exagerate
            // it a bit. Also, let convert it from 0..1 to 0..10 and avoid value 1 to
            // minimize component renderings.
            let audioVolume = Math.round(Math.pow(10, dBs / 85) * 10);

            if (audioVolume === 1) audioVolume = 0;
            audioLevel.set(audioVolume / 10);
         });

         // required, else the analyser wont work
         const audioElement = document.createElement('audio');
         audioElement.muted = true;
         audioElement.srcObject = stream;

         document.body.appendChild(audioElement);
         audioElement.play();

         return () => {
            document.body.removeChild(audioElement);
            analyser.stop();
         };
      }
   }, [stream]);

   const transform = useTransform(audioLevel, [0, 1], [0, 2]);
   const currentAudioLevel = useSpring(transform);

   return currentAudioLevel;
}
