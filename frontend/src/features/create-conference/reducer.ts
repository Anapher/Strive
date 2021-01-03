import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import * as conferenceServices from 'src/services/api/conference';
import { ConferenceData, CreateConferenceResponse, ConferencePermissions } from './types';

export type CreateConferenceState = {
   dialogOpen: boolean;
   isCreating: boolean;
   createdConferenceId: string | null;
   defaultPermissions: ConferencePermissions | null;
};

const initialState: CreateConferenceState = {
   dialogOpen: false,
   isCreating: false,
   createdConferenceId: null,
   defaultPermissions: null,
};

export const createConferenceAsync = createAsyncThunk('createConference/create', async (dto: ConferenceData) => {
   return await conferenceServices.create(dto);
});

export const loadDefaultPermissionsAsync = createAsyncThunk('createConference/loadDefaultPermissions', async () => {
   return await conferenceServices.getDefaultPermissions();
});

const createConference = createSlice({
   name: 'createConference',
   initialState,
   reducers: {
      openCreateDialog(state) {
         state.dialogOpen = true;
         state.createdConferenceId = null;
      },
      closeCreateDialog(state) {
         state.dialogOpen = false;
      },
   },
   extraReducers: {
      [createConferenceAsync.pending.type]: (state) => {
         state.isCreating = true;
      },
      [createConferenceAsync.rejected.type]: (state) => {
         state.isCreating = false;
      },
      [createConferenceAsync.fulfilled.type]: (state, action: PayloadAction<CreateConferenceResponse>) => {
         state.isCreating = false;
         state.createdConferenceId = action.payload.conferenceId;
      },
      [loadDefaultPermissionsAsync.fulfilled.type]: (state, action: PayloadAction<ConferencePermissions>) => {
         state.defaultPermissions = action.payload;
      },
   },
});

export const { openCreateDialog, closeCreateDialog } = createConference.actions;
export default createConference.reducer;
