import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import { events, fetchPermissions } from 'src/core-hub';
import { ParticipantPermissionInfo, Permissions } from 'src/core-hub.types';
import { connectSignal, onConnected, onConnectionError, onEventOccurred } from 'src/store/signal/actions';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { ConferenceInfo, ParticipantDto, TemporaryPermissions } from './types';

export type ConferenceState = {
   conferenceId: string | null;
   connectionError: DomainError | null;
   participants: ParticipantDto[] | null;
   conferenceState: ConferenceInfo | null;
   myPermissions: Permissions | null;
   participantsOpen: boolean;
   tempPermissions: TemporaryPermissions | null;

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
};

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
      ...createSynchronizeObjectReducer(['participants', 'conferenceState', 'tempPermissions']),
   },
});

export const { setParticipantsOpen, closePermissionDialog } = conferenceSlice.actions;

export default conferenceSlice.reducer;
