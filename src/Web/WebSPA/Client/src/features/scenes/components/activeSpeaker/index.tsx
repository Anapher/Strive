import React from 'react';
import { ActiveSpeakerScene, AvailableSceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import RecordVoiceOverIcon from '@material-ui/icons/RecordVoiceOver';
import RenderActiveSpeaker from './RenderActiveSpeaker';
import { useTranslation } from 'react-i18next';

function AvailableSceneListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();
   return <SceneListItem {...props} title={t('conference.scenes.active_speaker')} icon={<RecordVoiceOverIcon />} />;
}

const presenter: ScenePresenter<ActiveSpeakerScene> = {
   type: 'activeSpeaker',
   AvailableSceneListItem,
   RenderScene: RenderActiveSpeaker,
   getAutoHideMediaControls: () => true,
};

export default presenter;
