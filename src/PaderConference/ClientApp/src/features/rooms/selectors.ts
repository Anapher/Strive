import { RoomViewModel } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import _ from 'lodash';

export const selectRooms = createSelector(
   (state: RootState) => state.rooms,
   (state) => {
      if (!state.synchronized) return undefined;

      const { defaultRoomId, participants, rooms } = state.synchronized;

      return _.sortBy(
         rooms.map<RoomViewModel>((room) => ({
            ...room,
            isDefaultRoom: defaultRoomId === room.roomId,
            participants: Object.entries(participants)
               .filter((x) => x[1] === room.roomId)
               .map((x) => x[0]),
         })),
         (x) => x.isDefaultRoom,
         (x) => x.displayName,
      );
   },
);
