import PollIcon from '@material-ui/icons/Poll';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import PollOptionsPopper from './PollOptionsPopper';

export default function PollListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();

   return (
      <SceneListItemWithPopper {...props} icon={<PollIcon />} title={t('conference.scenes.poll.scene_label')}>
         <PollOptionsPopper />
      </SceneListItemWithPopper>
   );
}
