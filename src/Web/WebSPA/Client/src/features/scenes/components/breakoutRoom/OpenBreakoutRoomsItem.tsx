import { MenuItem } from '@material-ui/core';
import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { ActionListItemProps } from 'src/features/scenes/types';
import { setCreationDialogOpen } from '../../../breakout-rooms/reducer';
import { selectIsBreakoutRoomsOpen } from '../../../breakout-rooms/selectors';

export default function OpenBreakoutRoomsItem({ onClose }: ActionListItemProps) {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleOpen = () => {
      dispatch(setCreationDialogOpen(true));
      onClose();
   };

   const isOpen = useSelector(selectIsBreakoutRoomsOpen);

   return (
      <MenuItem id="scene-management-actions-breakoutrooms" onClick={handleOpen} disabled={isOpen}>
         <GroupWorkIcon fontSize="small" style={{ marginRight: 16 }} />
         {t('conference.scenes.breakout_rooms.label')}
      </MenuItem>
   );
}
