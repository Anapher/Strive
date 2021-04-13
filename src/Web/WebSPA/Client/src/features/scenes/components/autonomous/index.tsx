import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import StarIcon from '@material-ui/icons/Star';
import AutonomousScene from './AutonomousScene';
import { useTranslation } from 'react-i18next';

function ListItem(props: SceneListItemProps) {
   const { t } = useTranslation();
   return <SceneListItem {...props} title={t('conference.scenes.autonomous')} icon={<StarIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'autonomous',
   ListItem,
   RenderScene: AutonomousScene,
};

export default presenter;
