import { MenuItem } from '@material-ui/core';
import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { ActiveSceneMenuItemProps } from 'src/features/scenes/types';
import { setCreationDialogOpen } from '../breakoutRoomsSlice';
import { selectIsBreakoutRoomsOpen } from '../selectors';

export function OpenBreakoutRoomsItem({ onClose }: ActiveSceneMenuItemProps) {
   const dispatch = useDispatch();

   const handleOpen = () => {
      dispatch(setCreationDialogOpen(true));
      onClose();
   };

   const isOpen = useSelector(selectIsBreakoutRoomsOpen);

   return (
      <MenuItem onClick={handleOpen} disabled={isOpen}>
         <GroupWorkIcon fontSize="small" style={{ marginRight: 16 }} />
         Breakout Rooms
      </MenuItem>
   );
}
