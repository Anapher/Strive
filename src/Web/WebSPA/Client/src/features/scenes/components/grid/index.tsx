import React from 'react';
import { AvailableSceneListItemProps, GridScene, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import AppsIcon from '@material-ui/icons/Apps';
import ParticipantsGrid from './ParticipantsGrid';
import { useTranslation } from 'react-i18next';

function AvailableSceneListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();
   return <SceneListItem {...props} title={t('conference.scenes.grid')} icon={<AppsIcon />} />;
}

const presenter: ScenePresenter<GridScene> = {
   type: 'grid',
   AvailableSceneListItem,
   RenderScene: ParticipantsGrid,
   getAutoHideMediaControls: () => true,
};

export default presenter;
