import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';

export const selectBreakoutRoomState = (state: RootState) => state.breakoutRooms.synchronized?.active;

export const selectIsBreakoutRoomsOpen = createSelector(selectBreakoutRoomState, (state) => {
   return !!state;
});
