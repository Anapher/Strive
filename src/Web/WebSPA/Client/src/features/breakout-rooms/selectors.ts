import { RootState } from 'src/store';

export const selectBreakoutRoomState = (state: RootState) => state.breakoutRooms.synchronized?.active;
export const selectIsBreakoutRoomsOpen = (state: RootState) => Boolean(selectBreakoutRoomState(state));
