import { Menu, MenuItem } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { selectWhiteboard } from 'src/features/whiteboard/selectors';
import { RootState } from 'src/store';
import { ListItemPopperProps } from '../SceneListItemWithPopper';
import * as coreHub from 'src/core-hub';

export default function PollOptionsPopper({ anchorEl, scene, onClose, open }: ListItemPopperProps) {
   if (scene.type !== 'whiteboard') throw new Error('Invalid scene');

   const { t } = useTranslation();
   const whiteboard = useSelector((state: RootState) => selectWhiteboard(state, scene.id));
   const dispatch = useDispatch();

   if (!whiteboard) return null;

   const handleDelete = () => {
      dispatch(coreHub.whiteboardDelete(scene.id));
      onClose();
   };

   const handleToggleWhoCanEdit = () => {
      dispatch(
         coreHub.whiteboardUpdateSettings({
            whiteboardId: scene.id,
            settings: { anyoneCanEdit: !whiteboard.anyoneCanEdit },
         }),
      );
      onClose();
   };

   return (
      <Menu onClose={onClose} open={open} anchorEl={anchorEl as any}>
         <MenuItem onClick={handleToggleWhoCanEdit}>
            {t(
               whiteboard.anyoneCanEdit
                  ? 'conference.whiteboard.disallow_anyone'
                  : 'conference.whiteboard.allow_anyone',
            )}
         </MenuItem>
         <MenuItem onClick={handleDelete}>{t('common:delete')}</MenuItem>
      </Menu>
   );
}
