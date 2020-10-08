import { createSlice } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/conference-signal/synchronized-object';

type SynchronizedMedia = {
   isScreenshareActivated: boolean;
   partipantScreensharing: string;
};

export type ConferenceState = {
   synchronized: SynchronizedMedia | null;
};

const initialState: ConferenceState = {
   synchronized: null,
};

const mediaSlice = createSlice({
   name: 'media',
   initialState,
   reducers: {
      test(state) {
         state.synchronized = null;
      },
   },
   extraReducers: {
      ...createSynchronizeObjectReducer({ name: 'media', stateName: 'synchronized' }),
   },
});

export default mediaSlice.reducer;
