import { useTranslation } from 'react-i18next';
import { ActiveDescriptorProps, ScenePresenter, TalkingStickScene } from '../../types';
import ModeSceneListItem from './ModeSceneListItem';
import RenderTalkingStick from './RenderTalkingStick';

function ActiveDescriptor({ scene }: ActiveDescriptorProps<TalkingStickScene>) {
   const { t } = useTranslation();
   return (
      <span>
         {t('conference.scenes.talking_stick')} ({t(`conference.scenes.talking_stick_modes.${scene.mode}`)})
      </span>
   );
}

const presenter: ScenePresenter<TalkingStickScene> = {
   type: 'talkingStick',
   ModeSceneListItem,
   RenderScene: RenderTalkingStick,
   getAutoHideMediaControls: () => false,
   ActiveDescriptor,
};

export default presenter;
