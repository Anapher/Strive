import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React from 'react';
import { SceneListItemProps } from '../../types';
import SceneListItemWithPopper from '../SceneListItemWithPopper';
import BreakoutRoomsPopper from './BreakoutRoomsPopper';

export default function BreakoutRoomListItem(props: SceneListItemProps) {
   return (
      <SceneListItemWithPopper {...props} icon={<GroupWorkIcon />} title="Breakout Rooms">
         <BreakoutRoomsPopper />
      </SceneListItemWithPopper>
   );
}
