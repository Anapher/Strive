import { createSlice } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/conference-signal/synchronized-object';
import { ConferenceParticipantStreamInfo } from './types';

export type MediaState = {
   streams: ConferenceParticipantStreamInfo | null;
};

const initialState: MediaState = {
   streams: null,
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
   },
});

export default mediaSlice.reducer;
