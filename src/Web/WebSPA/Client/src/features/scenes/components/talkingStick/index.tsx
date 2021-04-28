import { ScenePresenter, TalkingStickScene } from '../../types';
import ModeSceneListItem from './ModeSceneListItem';
import RenderTalkingStick from './RenderTalkingStick';

const presenter: ScenePresenter<TalkingStickScene> = {
   type: 'talkingStick',
   ModeSceneListItem,
   RenderScene: RenderTalkingStick,
   getAutoHideMediaControls: () => false,
};

export default presenter;
