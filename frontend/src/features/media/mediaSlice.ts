import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { onEventOccurred } from 'src/store/signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { ConferenceParticipantStreamInfo, ParticipantEquipmentStatusDto } from './types';
import { events } from 'src/core-hub';

export type MediaState = {
   streams: ConferenceParticipantStreamInfo | null;
   equipment: ParticipantEquipmentStatusDto | null;
};

const initialState: MediaState = {
   streams: null,
   equipment: null,
};

const mediaSlice = createSlice({
   name: 'media',
   initialState,
   reducers: {
      test(state) {
         state.streams = null;
      },
   },
   extraReducers: {
      ...createSynchronizeObjectReducer([{ name: 'mediaStreams', stateName: 'streams' }]),
      [onEventOccurred(events.onEquipmentUpdated).type]: (
         state,
         { payload }: PayloadAction<ParticipantEquipmentStatusDto>,
      ) => {
         state.equipment = payload;
      },
   },
});

export default mediaSlice.reducer;
