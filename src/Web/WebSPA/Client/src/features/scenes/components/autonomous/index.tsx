import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import StarIcon from '@material-ui/icons/Star';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { ModeSceneListItemProps, ScenePresenter } from '../../types';
import AutonomousScene from './AutonomousScene';

function ModeSceneListItem({ selectedScene, onChangeScene }: ModeSceneListItemProps) {
   const { t } = useTranslation();

   const isSelected = selectedScene.type === 'autonomous';
   const handleSetScene = () => onChangeScene({ type: 'autonomous' });

   return (
      <ListItem button selected={isSelected} onClick={handleSetScene}>
         <ListItemIcon>
            <StarIcon />
         </ListItemIcon>
         <ListItemText
            primary={t('conference.scenes.autonomous')}
            secondary={t('conference.scenes.autonomous_description')}
         />
      </ListItem>
   );
}

const presenter: ScenePresenter = {
   type: 'autonomous',
   ModeSceneListItem,
   RenderScene: AutonomousScene,
};

export default presenter;
