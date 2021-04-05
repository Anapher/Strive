import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import RecordVoiceOverIcon from '@material-ui/icons/RecordVoiceOver';
import ActiveSpeakerScene from './ActiveSpeakerScene';

function ListItem(props: SceneListItemProps) {
   return <SceneListItem {...props} title="Active Speaker" icon={<RecordVoiceOverIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'activeSpeaker',
   ListItem,
   RenderScene: ActiveSpeakerScene,
};

export default presenter;
