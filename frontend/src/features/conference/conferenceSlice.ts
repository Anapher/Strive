import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { events } from 'src/core-hub';
import { connectSignal, onConnected, onConnectionError, onEventOccurred } from 'src/store/signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { IRestError } from 'src/utils/error-result';
import { ConferenceType, Permissions } from '../create-conference/types';
import { ParticipantDto } from './types';

export type ConferenceInfo = {
   conferenceState: 'active' | 'inactive';
   scheduledDate?: string;
   isOpen: boolean;
   conferenceType?: ConferenceType;
   permissions: Permissions;
   moderators: string[];
};

export type ConferenceState = {
   conferenceId: string | null;
   connectionError: IRestError | null;
   participants: ParticipantDto[] | null;
   conferenceState: ConferenceInfo | null;
   myPermissions: Permissions | null;
   participantsOpen: boolean;
};

const initialState: ConferenceState = {
   conferenceId: null,
   connectionError: null,
   participants: null,
   conferenceState: null,
   myPermissions: null,
   participantsOpen: true,
};

const conferenceSlice = createSlice({
   name: 'conference',
   initialState,
   reducers: {
      setParticipantsOpen(state, { payload }: PayloadAction<boolean>) {
         state.participantsOpen = payload;
      },
   },
   extraReducers: {
      [connectSignal.type]: (state) => {
         state.conferenceId = null;
         state.connectionError = null;
         state.participants = null;
      },
      [onConnected.type]: (state, action) => {
         state.conferenceId = action.conferenceId;
      },
      [onConnectionError.type]: (state, action) => {
         state.connectionError = action.payload.error;
      },
      [onEventOccurred(events.onPermissionsUpdated).type]: (state, action: PayloadAction<Permissions>) => {
         state.myPermissions = action.payload;
      },
      ...createSynchronizeObjectReducer(['participants', 'conferenceState']),
   },
});

export const { setParticipantsOpen } = conferenceSlice.actions;

export default conferenceSlice.reducer;
