import React from 'react';
import { SceneListItemProps, ScenePresenter } from '../../types';
import SceneListItem from '../SceneListItem';
import AppsIcon from '@material-ui/icons/Apps';
import ParticipantsGrid from './ParticipantsGrid';
import { useTranslation } from 'react-i18next';

function ListItem(props: SceneListItemProps) {
   const { t } = useTranslation();

   return <SceneListItem {...props} title={t('conference.scenes.grid')} icon={<AppsIcon />} />;
}

const presenter: ScenePresenter = {
   type: 'grid',
   ListItem,
   RenderScene: ParticipantsGrid,
};

export default presenter;
