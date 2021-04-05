import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import AppsIcon from '@material-ui/icons/Apps';
import ParticipantsGrid from './ParticipantsGrid';

function ListItem(props: SceneListItemProps) {
   return <SceneListItem {...props} title="Grid" icon={<AppsIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'grid',
   ListItem,
   RenderScene: ParticipantsGrid,
};

export default presenter;
