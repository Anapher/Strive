import PollIcon from '@material-ui/icons/Poll';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import PollOptionsPopper from './PollOptionsPopper';
import MoreVertIcon from '@material-ui/icons/MoreVert';

export default function PollListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();

   return (
      <SceneListItemWithPopper
         PopperComponent={PollOptionsPopper}
         {...props}
         icon={<PollIcon />}
         listItemIcon={<MoreVertIcon />}
         title={t('conference.scenes.poll.scene_label')}
      />
   );
}
