import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { EQUIPMENT, MEDIA, SynchronizedEquipment } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { ParticipantPayloadAction } from 'src/types';
import { ParticipantAudioInfo, SynchronizedMediaState } from './types';

export type MediaState = {
   synchronized: SynchronizedMediaState | null;
   equipment: SynchronizedEquipment | null;
   participantAudio: { [id: string]: ParticipantAudioInfo | undefined };
   userInteractionMade: boolean;
};

const initialState: MediaState = {
   userInteractionMade: false,
   synchronized: null,
   equipment: null,
   participantAudio: {},
};

const mediaSlice = createSlice({
   name: 'media',
   initialState,
   reducers: {
      setParticipantAudio(state, { payload: { data, participantId } }: ParticipantPayloadAction<ParticipantAudioInfo>) {
         state.participantAudio[participantId] = data;
      },
      removeParticipantAudio(state, { payload }: PayloadAction<string>) {
         delete state.participantAudio[payload];
      },
      patchParticipantAudio(
         state,
         { payload: { data, participantId } }: ParticipantPayloadAction<Partial<ParticipantAudioInfo>>,
      ) {
         const info = state.participantAudio[participantId];
         if (info) state.participantAudio[participantId] = { ...info, ...data };
      },
      userInteractionMade(state) {
         state.userInteractionMade = true;
      },
   },
   extraReducers: {
      ...synchronizeObjectState([
         { type: 'exactId', syncObjId: MEDIA, propertyName: 'synchronized' },
         { type: 'single', baseId: EQUIPMENT, propertyName: 'equipment' },
      ]),
   },
});

export const {
   setParticipantAudio,
   removeParticipantAudio,
   patchParticipantAudio,
   userInteractionMade,
} = mediaSlice.actions;

export default mediaSlice.reducer;
