import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { onEventOccurred } from 'src/store/signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { ConferenceParticipantStreamInfo, ConnectedEquipmentDto } from './types';
import { events } from 'src/core-hub';

export type MediaState = {
   streams: ConferenceParticipantStreamInfo | null;
   equipment: ConnectedEquipmentDto[] | null;
};

const initialState: MediaState = {
   streams: null,
   equipment: null,
};

const mediaSlice = createSlice({
   name: 'media',
   initialState,
   reducers: {},
   extraReducers: {
      ...createSynchronizeObjectReducer([{ name: 'mediaStreams', stateName: 'streams' }]),
      [onEventOccurred(events.onEquipmentUpdated).type]: (
         state,
         { payload }: PayloadAction<ConnectedEquipmentDto[]>,
      ) => {
         state.equipment = payload;
      },
   },
});

export default mediaSlice.reducer;
