import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import { fetchPermissions } from 'src/core-hub';
import { ParticipantPermissionInfo } from 'src/core-hub.types';
import * as conferenceLinks from 'src/services/api/conference-link';
import { connectSignal, onConnected, onConnectionError } from 'src/store/signal/actions';
import {
   CONFERENCE,
   PARTICIPANTS,
   PARTICIPANT_PERMISSIONS,
   SynchronizedConferenceInfo,
   SynchronizedParticipants,
   SynchronizedParticipantsPermissions,
   TEMPORARY_PERMISSIONS,
} from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { serializeRequestError } from 'src/utils/error-result';
import { ConferenceLink, TemporaryPermissions } from './types';

export type ConferenceState = {
   conferenceId: string | null;
   connectionError: DomainError | null;
   participants: SynchronizedParticipants | null;
   conferenceState: SynchronizedConferenceInfo | null;
   myPermissions: SynchronizedParticipantsPermissions | null;
   participantsOpen: boolean;
   temporaryPermissions: TemporaryPermissions | null;

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
   temporaryPermissions: null,
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
      ...synchronizeObjectState([
         { type: 'exactId', syncObjId: PARTICIPANTS, propertyName: 'participants' },
         { type: 'exactId', syncObjId: CONFERENCE, propertyName: 'conferenceState' },
         { type: 'single', baseId: PARTICIPANT_PERMISSIONS, propertyName: 'myPermissions' },
         { type: 'exactId', syncObjId: TEMPORARY_PERMISSIONS, propertyName: 'temporaryPermissions' },
      ]),
   },
});

export const { setParticipantsOpen, closePermissionDialog } = conferenceSlice.actions;

export default conferenceSlice.reducer;
