import { createSlice } from '@reduxjs/toolkit';
import { onConferenceJoined } from 'src/store/conference-signal/actions';

export type ConferenceState = {
   conferenceId: string | null;
};

const initialState: ConferenceState = {
   conferenceId: null,
};

const conferenceSlice = createSlice({
   name: 'conference',
   initialState,
   reducers: {
      test(state) {
         state.conferenceId = null;
      },
   },
   extraReducers: {
      [onConferenceJoined.type]: (state, action) => {
         state.conferenceId = action.conferenceId;
      },
   },
});

export default conferenceSlice.reducer;
