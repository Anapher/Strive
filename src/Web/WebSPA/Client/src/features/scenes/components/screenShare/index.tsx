import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectParticipant } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import { AvailableSceneListItemProps, ScenePresenter, ScreenShareScene } from '../../types';
import SceneListItem from '../SceneListItem';
import ScreenShare from './ScreenShare';

function AvailableSceneListItem(props: AvailableSceneListItemProps<ScreenShareScene>) {
   const { t } = useTranslation();
   const name = useSelector((state: RootState) => selectParticipant(state, props.scene.participantId))?.displayName;

   return (
      <SceneListItem {...props} title={t('conference.scenes.screen_share', { name })} icon={<DesktopWindowsIcon />} />
   );
}

const presenter: ScenePresenter<ScreenShareScene> = {
   type: 'screenShare',
   AvailableSceneListItem,
   RenderScene: ScreenShare,
   getSceneId: (scene) => `${scene.type}::${scene.participantId}`,
   getAutoHideMediaControls: (scene, participantId) => scene.participantId !== participantId,
};

export default presenter;
