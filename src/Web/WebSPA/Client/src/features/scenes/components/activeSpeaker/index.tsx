import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import RecordVoiceOverIcon from '@material-ui/icons/RecordVoiceOver';
import ActiveSpeakerScene from './ActiveSpeakerScene';
import { useTranslation } from 'react-i18next';

function ListItem(props: SceneListItemProps) {
   const { t } = useTranslation();
   return <SceneListItem {...props} title={t('conference.scenes.active_speaker')} icon={<RecordVoiceOverIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'activeSpeaker',
   ListItem,
   RenderScene: ActiveSpeakerScene,
};

export default presenter;
