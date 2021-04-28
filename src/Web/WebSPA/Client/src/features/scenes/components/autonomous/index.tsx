import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import StarIcon from '@material-ui/icons/Star';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AutonomousScene, ModeSceneListItemProps, ScenePresenter } from '../../types';
import RenderAutonomous from './RenderAutonomous';

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

function ActiveDescriptor() {
   const { t } = useTranslation();
   return <span>{t('conference.scenes.autonomous')}</span>;
}

const presenter: ScenePresenter<AutonomousScene> = {
   type: 'autonomous',
   ModeSceneListItem,
   RenderScene: RenderAutonomous,
   ActiveDescriptor,
};

export default presenter;
