import { RoomViewModel } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import _ from 'lodash';
import { selectAccessToken, selectMyParticipantId } from '../auth/selectors';

export const selectRooms = (state: RootState) => state.rooms.synchronized;

export const selectParticipantRoom = createSelector(selectRooms, selectAccessToken, (rooms, token) => {
   if (!token) return undefined;
   return rooms?.participants[token.nameid];
});

export const selectParticipantsOfCurrentRoom = createSelector(selectParticipantRoom, selectRooms, (room, rooms) => {
   if (!rooms) return [];

   return Object.entries(rooms.participants)
      .filter(([, roomId]) => roomId === room)
      .map<string>(([participantId]) => participantId);
});

export const selectParticipantsOfCurrentRoomWithoutMe = createSelector(
   selectParticipantsOfCurrentRoom,
   selectMyParticipantId,
   (participants, participantId) => {
      return participants.filter((x) => x !== participantId);
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
