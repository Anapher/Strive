import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { onEventOccurred } from 'src/store/signal/actions';
import { ConferenceParticipantStreamInfo, ConnectedEquipmentDto, ParticipantAudioInfo } from './types';
import { events } from 'src/core-hub';
import { ParticipantPayloadAction } from 'src/types';

export type MediaState = {
   streams: ConferenceParticipantStreamInfo | null;
   equipment: ConnectedEquipmentDto[] | null;
   participantAudio: { [id: string]: ParticipantAudioInfo | undefined };
   userInteractionMade: boolean;
};

const initialState: MediaState = {
   userInteractionMade: false,
   streams: null,
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
      [onEventOccurred(events.onEquipmentUpdated).type]: (
         state,
         { payload }: PayloadAction<ConnectedEquipmentDto[]>,
      ) => {
         state.equipment = payload;
      },
   },
});

export const {
   setParticipantAudio,
   removeParticipantAudio,
   patchParticipantAudio,
   userInteractionMade,
} = mediaSlice.actions;

export default mediaSlice.reducer;
