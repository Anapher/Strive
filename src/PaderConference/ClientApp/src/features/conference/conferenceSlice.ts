import { createSlice } from '@reduxjs/toolkit';
import { joinConference, onConferenceJoined, onConferenceJoinFailed } from 'src/store/conference-signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/conference-signal/synchronized-object';
import { ParticipantDto } from 'src/store/conference-signal/types';
import { IRestError } from 'src/utils/error-result';

export type ConferenceState = {
   conferenceId: string | null;
   connectionError: IRestError | null;
   participants: ParticipantDto[] | null;
};

const initialState: ConferenceState = {
   conferenceId: null,
   connectionError: null,
   participants: null,
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
      [joinConference.type]: (state) => {
         state.conferenceId = null;
         state.connectionError = null;
         state.participants = null;
      },
      [onConferenceJoined.type]: (state, action) => {
         state.conferenceId = action.conferenceId;
      },
      [onConferenceJoinFailed.type]: (state, action) => {
         state.connectionError = action.payload;
      },
      ...createSynchronizeObjectReducer('participants'),
   },
});

export default conferenceSlice.reducer;
