import useSound from 'use-sound';
import testAudioFile from 'src/assets/audio/test_audio_file.mp3';
import striveConferenceOpened from 'src/assets/audio/strive_conference_opened.mp3';
import { ReturnedValue } from 'use-sound/dist/types';

const availableSounds = {
   testAudioFile,
   striveConferenceOpened,
};

export type AvailableSound = keyof typeof availableSounds;

export default function useStriveSound(sound: AvailableSound): ReturnedValue {
   return useSound(availableSounds[sound], { volume: 0.75 });
}
