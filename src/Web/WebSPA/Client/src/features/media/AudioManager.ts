import { MotionValue } from 'framer-motion';
import hark from 'hark';
import { TypedEmitter } from 'tiny-typed-emitter';
import { ParticipantAudioElement } from './types';

interface AudioManagerEvents {
   update: (participantId: string) => void;
}

export default class AudioManager extends TypedEmitter<AudioManagerEvents> {
   public audioElems = new Map<string, ParticipantAudioElement>();

   register(
      elem: HTMLAudioElement,
      participantId: string,
      track: MediaStreamTrack,
      onSpeaking: () => void,
      onSpeakingStopped: () => void,
      volume: number,
      muted: boolean,
      speakingThreshold = 2,
   ) {
      elem.volume = volume;
      elem.muted = muted;

      if (this.audioElems.get(participantId)?.elem === elem) return;

      const stream = new MediaStream();
      stream.addTrack(track);

      const audioLevel = new MotionValue<number>(0);
      let speaking = false;

      const analyser = hark(stream, { play: false });
      analyser.on('volume_change', (dBs) => {
         // The exact formula to convert from dBs (-100..0) to linear (0..1) is:
         //   Math.pow(10, dBs / 20)
         // However it does not produce a visually useful output, so let exagerate
         // it a bit. Also, let convert it from 0..1 to 0..10 and avoid value 1 to
         // minimize component renderings.
         let audioVolume = Math.round(Math.pow(10, dBs / 85) * 10);

         if (audioVolume === 1) audioVolume = 0;

         if (audioVolume >= speakingThreshold) {
            if (!speaking) {
               onSpeaking();
               speaking = true;
            }
         } else {
            if (speaking) {
               onSpeakingStopped();
               speaking = false;
            }
         }

         audioLevel.set(audioVolume / 10);
      });

      elem.srcObject = stream;
      elem.play();

      this.audioElems.set(participantId, { audioLevel, elem, stream, hark: analyser });
      this.emit('update', participantId);
   }

   unregister(participantId: string) {
      const audioElem = this.audioElems.get(participantId);
      if (audioElem) {
         this.audioElems.delete(participantId);
         audioElem.hark.stop();
         this.emit('update', participantId);
      }
   }
}
