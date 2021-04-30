import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { selectMyParticipantId } from '../auth/selectors';
import { Participant } from './types';

const selectParticipantsMapRaw = (state: RootState) => state.conference.participants?.participants ?? {};

export type ParticipantsMap = { [participantId: string]: Participant };
export const selectParticipants = createSelector(
   selectParticipantsMapRaw,
   (participants) =>
      Object.fromEntries(Object.entries(participants).map(([id, data]) => [id, { ...data, id }])) as ParticipantsMap,
);

export const selectParticipant: (state: RootState, participantId: string | undefined) => Participant | undefined = (
   state: RootState,
   participantId: string | undefined,
) => {
   if (!participantId) return undefined;

   const participantMap = selectParticipants(state);
   return participantMap[participantId];
};

export const selectParticipantList = createSelector(selectParticipants, (participants) =>
   Object.entries(participants).map(([id, data]) => ({ ...data, id })),
);

export const selectOtherParticipants = createSelector(
   selectParticipants,
   selectMyParticipantId,
   (participants, myId) => {
      return Object.fromEntries(Object.entries(participants).filter(([id]) => id !== myId));
   },
);

export const selectParticipantTempPermissions = (state: RootState, participantId: string) =>
   state.conference.temporaryPermissions?.assigned[participantId];
