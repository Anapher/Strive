import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { setCreationDialogOpen } from '../breakoutRoomsSlice';
import CreateBreakoutRoomsDialog from './CreateBreakoutRoomsDialog';
import UpdateBreakoutRoomsDialog from './UpdateBreakoutRoomsDialog';

export default function OpenBreakoutRoomsDialog() {
   const dispatch = useDispatch();
   const open = useSelector((state: RootState) => state.breakoutRooms.creationDialogOpen);
   const breakoutRoomState = useSelector((state: RootState) => state.breakoutRooms.synchronized?.active);

   const handleClose = () => dispatch(setCreationDialogOpen(false));

   return breakoutRoomState ? (
      <UpdateBreakoutRoomsDialog onClose={handleClose} open={open} active={breakoutRoomState} />
   ) : (
      <CreateBreakoutRoomsDialog onClose={handleClose} open={open} />
   );
}
