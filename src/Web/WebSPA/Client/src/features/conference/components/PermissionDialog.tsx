import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { closePermissionDialog } from '../reducer';
import { selectParticipant } from '../selectors';
import PermissionsView from './PermissionsView';

export default function PermissionDialog() {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const data = useSelector((state: RootState) => state.conference.permissionDialogData);
   const open = useSelector((state: RootState) => state.conference.permissionDialogOpen);
   const participant = useSelector((state: RootState) => selectParticipant(state, data?.participantId));
   const handleCloseDialog = () => dispatch(closePermissionDialog());

   return (
      <Dialog open={open} onClose={handleCloseDialog} fullWidth maxWidth="md">
         <DialogTitle>{t('conference.dialog_permissions.title', { name: participant?.displayName })}</DialogTitle>
         <DialogContent>{data && <PermissionsView permissions={data} />}</DialogContent>
      </Dialog>
   );
}
