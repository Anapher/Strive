import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { AvailableSceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import BreakoutRoomsPopper from './BreakoutRoomsPopper';
import SettingsIcon from '@material-ui/icons/Settings';

export default function BreakoutRoomListItem(props: AvailableSceneListItemProps) {
   const { t } = useTranslation();

   return (
      <SceneListItemWithPopper
         {...props}
         icon={<GroupWorkIcon />}
         listItemIcon={<SettingsIcon />}
         title={t('conference.scenes.breakout_rooms.label')}
         PopperComponent={BreakoutRoomsPopper}
      />
   );
}
