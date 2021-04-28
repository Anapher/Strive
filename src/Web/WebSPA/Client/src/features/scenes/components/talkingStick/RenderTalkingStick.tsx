import { RenderSceneProps, TalkingStickScene } from '../../types';
import TalkingStickQueue from './TalkingStickQueue';
import TalkingStickRace from './TalkingStickRace';
import TalkingStickSpeakerPassingStick from './TalkingStickSpeakerPassingStick';

export default function RenderTalkingStick({ scene, ...props }: RenderSceneProps<TalkingStickScene>) {
   switch (scene.mode) {
      case 'queue':
         return <TalkingStickQueue {...props} scene={scene} />;
      case 'race':
         return <TalkingStickRace {...props} scene={scene} />;
      case 'moderated':
      case 'speakerPassStick':
         return <TalkingStickSpeakerPassingStick {...props} scene={scene} />;
      default:
         return null;
   }
}
