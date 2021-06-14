import { MenuItem } from '@material-ui/core';
import { Draw } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { ActionListItemProps } from 'src/features/scenes/types';

export default function WhiteboardActionItem({ onClose }: ActionListItemProps) {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleOpen = () => {
      dispatch(coreHub.createWhiteboard());
      onClose();
   };

   return (
      <MenuItem id="scene-management-actions-whiteboard" onClick={handleOpen}>
         <Draw fontSize="small" style={{ marginRight: 16 }} />
         Whiteboard
      </MenuItem>
   );
}
