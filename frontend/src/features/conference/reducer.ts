import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import { events, fetchPermissions } from 'src/core-hub';
import { ParticipantPermissionInfo, Permissions } from 'src/core-hub.types';
import { connectSignal, onConnected, onConnectionError, onEventOccurred } from 'src/store/signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { serializeRequestError } from 'src/utils/error-result';
import { ConferenceInfo, ConferenceLink, ParticipantDto, TemporaryPermissions } from './types';
import * as conferenceLinks from 'src/services/api/conference-link';

export type ConferenceState = {
   conferenceId: string | null;
   connectionError: DomainError | null;
   participants: ParticipantDto[] | null;
   conferenceState: ConferenceInfo | null;
   myPermissions: Permissions | null;
   participantsOpen: boolean;
   tempPermissions: TemporaryPermissions | null;

   conferenceLinks: ConferenceLink[] | null;

   permissionDialogData: ParticipantPermissionInfo | null;
   permissionDialogOpen: boolean;
};

const initialState: ConferenceState = {
   conferenceId: null,
   connectionError: null,
   participants: null,
   conferenceState: null,
   myPermissions: null,
   participantsOpen: true,
   permissionDialogData: null,
   permissionDialogOpen: false,
   tempPermissions: null,
   conferenceLinks: null,
};

export const fetchConferenceLinks = createAsyncThunk(
   'conference/fetchLinks',
   async () => {
      return await conferenceLinks.fetch();
   },
   {
      serializeError: serializeRequestError,
   },
);

const conferenceSlice = createSlice({
   name: 'conference',
   initialState,
   reducers: {
      setParticipantsOpen(state, { payload }: PayloadAction<boolean>) {
         state.participantsOpen = payload;
      },
      closePermissionDialog(state) {
         state.permissionDialogOpen = false;
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
      [fetchPermissions.returnAction]: (
         state,
         { payload }: PayloadAction<SuccessOrError<ParticipantPermissionInfo>>,
      ) => {
         if (payload.success) {
            state.permissionDialogData = payload.response;
            state.permissionDialogOpen = true;
         }
      },
      [fetchConferenceLinks.fulfilled.type]: (state, { payload }: PayloadAction<ConferenceLink[]>) => {
         state.conferenceLinks = payload;
      },
      ...createSynchronizeObjectReducer(['participants', 'conferenceState', 'tempPermissions']),
   },
});

export const { setParticipantsOpen, closePermissionDialog } = conferenceSlice.actions;

export default conferenceSlice.reducer;
