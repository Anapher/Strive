import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { AvailableSceneListItemProps, ScenePresenter, ScreenShareScene } from '../../types';
import SceneListItem from '../SceneListItem';
import ScreenShare from './ScreenShare';

function AvailableSceneListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();

   const participants = useSelector(selectParticipants);
   const name = participants.find((x) => x.id === (props.scene as ScreenShareScene).participantId)?.displayName;

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
