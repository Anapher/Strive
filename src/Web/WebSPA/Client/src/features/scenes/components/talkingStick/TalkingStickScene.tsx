import { RenderSceneProps } from '../../types';
import TalkingStickQueue from './TalkingStickQueue';

export default function TalkingStickScene({ scene, ...props }: RenderSceneProps) {
   if (scene.type !== 'talkingStick') return null;

   switch (scene.mode) {
      case 'queue':
         return <TalkingStickQueue {...props} scene={scene} />;
      default:
         return null;
   }
}
