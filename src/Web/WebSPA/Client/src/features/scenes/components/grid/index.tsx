import React from 'react';
import { AvailableSceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import AppsIcon from '@material-ui/icons/Apps';
import ParticipantsGrid from './ParticipantsGrid';
import { useTranslation } from 'react-i18next';

function AvailableSceneListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();
   return <SceneListItem {...props} title={t('conference.scenes.grid')} icon={<AppsIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'grid',
   AvailableSceneListItem,
   RenderScene: ParticipantsGrid,
};

export default presenter;
