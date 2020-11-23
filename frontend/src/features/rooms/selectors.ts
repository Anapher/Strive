import { RoomViewModel } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import _ from 'lodash';
import { selectAccessToken } from '../auth/selectors';

export const selectRooms = (state: RootState) => state.rooms.synchronized;

export const selectParticipantRoom = createSelector(selectRooms, selectAccessToken, (rooms, token) => {
   if (!token) return undefined;
   return rooms?.participants[token.nameid];
});

export const selectParticipantsOfCurrentRoom = createSelector(
   selectParticipantRoom,
   selectRooms,
   selectAccessToken,
   (room, rooms, token) => {
      if (!rooms) return [];

      return Object.entries(rooms.participants)
         .filter(([participantId, roomId]) => roomId === room && participantId !== token?.nameid)
         .map<string>(([participantId]) => participantId);
   },
);

export const selectRoomViewModels = createSelector(selectRooms, (state) => {
   if (!state) return undefined;

   const { defaultRoomId, participants, rooms } = state;

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
});
