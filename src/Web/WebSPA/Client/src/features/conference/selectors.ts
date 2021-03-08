import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { selectMyParticipantId } from '../auth/selectors';
import { Participant } from './types';

export const selectParticipants = (state: RootState): Participant[] =>
   state.conference.participants
      ? Object.entries(state.conference.participants.participants).map(([id, data]) => ({ ...data, id }))
      : [];

export const selectParticipant = (state: RootState, participantId: string) =>
   selectParticipants(state)?.find((x) => x.id === participantId);

export const selectOtherParticipants = createSelector(
   selectParticipants,
   selectMyParticipantId,
   (participants, myId) => {
      return participants?.filter((x) => x.id !== myId);
   },
);

export const selectParticipantTempPermissions = (state: RootState, participantId: string) =>
   state.conference.tempPermissions?.[participantId];
