import { Dialog } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { setCreationDialogOpen } from '../reducer';
import CreateBreakoutRoomsDialog from './CreateBreakoutRoomsDialog';
import UpdateBreakoutRoomsDialog from './UpdateBreakoutRoomsDialog';

export default function BreakoutRoomsDialog() {
   const dispatch = useDispatch();
   const open = useSelector((state: RootState) => state.breakoutRooms.creationDialogOpen);
   const active = useSelector((state: RootState) => state.breakoutRooms.synchronized?.active);

   const handleClose = () => dispatch(setCreationDialogOpen(false));

   return (
      <Dialog
         open={open}
         onClose={handleClose}
         aria-labelledby="form-dialog-title"
         fullWidth
         maxWidth="md"
         scroll="paper"
      >
         {active ? (
            <UpdateBreakoutRoomsDialog onClose={handleClose} active={active} />
         ) : (
            <CreateBreakoutRoomsDialog onClose={handleClose} />
         )}
      </Dialog>
   );
}
