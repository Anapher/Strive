import { createSlice } from '@reduxjs/toolkit';
import {
   joinConference,
   onConferenceJoined,
   onConferenceJoinFailed,
   onParticipantsUpdated,
} from 'src/store/conference-signal/actions';
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
      [onParticipantsUpdated.type]: (state, action) => {
         state.participants = action.payload;
      },
   },
});

export default conferenceSlice.reducer;
