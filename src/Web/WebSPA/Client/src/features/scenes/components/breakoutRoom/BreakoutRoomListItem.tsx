import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import BreakoutRoomsPopper from './BreakoutRoomsPopper';

export default function BreakoutRoomListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();

   return (
      <SceneListItemWithPopper {...props} icon={<GroupWorkIcon />} title={t('conference.scenes.breakout_rooms.label')}>
         <BreakoutRoomsPopper />
      </SceneListItemWithPopper>
   );
}
