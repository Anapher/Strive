import { createSlice } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/conference-signal/synchronized-object';
import { ConferenceParticipantStreamInfo } from './types';

export type MediaState = {
   streams: ConferenceParticipantStreamInfo | null;
   audioLevel: { [key: string]: number } | null;
};

const initialState: MediaState = {
   streams: null,
   audioLevel: null,
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
      ...createSynchronizeObjectReducer([
         { name: 'mediaStreams', stateName: 'streams' },
         { name: 'mediaAudioLevel', stateName: 'audioLevel' },
      ]),
   },
});

export default mediaSlice.reducer;
