import { SceneListItemProps, ScenePresenter, ScreenShareScene } from '../../types';
import SceneListItem from '../SceneListItem';
import React from 'react';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import ScreenShare from './ScreenShare';
import { useTranslation } from 'react-i18next';

function ListItem(props: SceneListItemProps) {
   const { t } = useTranslation();

   const participants = useSelector(selectParticipants);
   const name = participants.find((x) => x.id === (props.scene as ScreenShareScene).participantId)?.displayName;

   return (
      <SceneListItem {...props} title={t('conference.scenes.screen_share', { name })} icon={<DesktopWindowsIcon />} />
   );
}

const presenter: ScenePresenter = {
   type: 'screenShare',
   ListItem,
   RenderScene: ScreenShare,
};

export default presenter;
