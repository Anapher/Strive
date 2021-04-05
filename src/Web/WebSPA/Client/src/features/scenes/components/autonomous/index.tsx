import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import StarIcon from '@material-ui/icons/Star';
import AutonomousScene from './AutonomousScene';

function ListItem(props: SceneListItemProps) {
   return <SceneListItem {...props} title="Autonomous" icon={<StarIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'autonomous',
   ListItem,
   RenderScene: AutonomousScene,
};

export default presenter;
