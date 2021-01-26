import { RootState } from 'src/store';

export const selectMyParticipantId = (state: RootState) => state.auth.participantId;
