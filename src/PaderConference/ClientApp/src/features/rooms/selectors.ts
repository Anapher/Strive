import { RoomViewModel } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';

export const roomsSelector = createSelector(
   (state: RootState) => state.rooms,
   (state) => {
      if (!state.synchronized) return undefined;

      const { defaultRoomId, participants, rooms } = state.synchronized;

      return rooms.map<RoomViewModel>((room) => ({
         ...room,
         isDefaultRoom: defaultRoomId === room.roomId,
         participants: Object.entries(participants)
            .filter((x) => x[1] === room.roomId)
            .map((x) => x[0]),
      }));
   },
);
