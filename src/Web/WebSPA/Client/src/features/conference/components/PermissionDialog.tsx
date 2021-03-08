import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { closePermissionDialog } from '../reducer';
import { selectParticipants } from '../selectors';
import PermissionsView from './PermissionsView';

export default function PermissionDialog() {
   const data = useSelector((state: RootState) => state.conference.permissionDialogData);
   const open = useSelector((state: RootState) => state.conference.permissionDialogOpen);
   const participants = useSelector(selectParticipants);

   const dispatch = useDispatch();

   const handleCloseDialog = () => dispatch(closePermissionDialog());

   return (
      <Dialog open={open} onClose={handleCloseDialog} fullWidth maxWidth="md">
         <DialogTitle>
            Permissions of {participants?.find((x) => x.id === data?.participantId)?.displayName}
         </DialogTitle>
         <DialogContent>{data && <PermissionsView permissions={data} />}</DialogContent>
      </Dialog>
   );
}
