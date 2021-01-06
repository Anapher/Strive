import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError } from 'src/communication-types';
import * as conferenceServices from 'src/services/api/conference';
import { ConferenceData, CreateConferenceResponse, ConferencePermissions } from './types';

export type CreateConferenceState = {
   loadingConferenceDataError: DomainError | null;
   conferenceData: ConferenceData | null;

   dialogOpen: boolean;
   mode: 'create' | 'edit';

   isCreating: boolean;
   createdConferenceId: string | null;
};

const initialState: CreateConferenceState = {
   mode: 'create',
   loadingConferenceDataError: null,
   dialogOpen: false,
   isCreating: false,
   createdConferenceId: null,
   conferenceData: null,
};

export const createConferenceAsync = createAsyncThunk('createConference/create', async (dto: ConferenceData) => {
   return await conferenceServices.create(dto);
});

export const openDialogToCreateAsync = createAsyncThunk('createConference/openForCreate', async () => {
   return await conferenceServices.getDefault();
});

const createConference = createSlice({
   name: 'createConference',
   initialState,
   reducers: {
      closeDialog(state) {
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
      [openDialogToCreateAsync.pending.type]: (state) => {
         state.conferenceData = null;
         state.dialogOpen = true;
         state.createdConferenceId = null;
         state.loadingConferenceDataError = null;
         state.mode = 'create';
      },
      [openDialogToCreateAsync.fulfilled.type]: (state, action: PayloadAction<ConferenceData>) => {
         state.conferenceData = action.payload;
      },
      [openDialogToCreateAsync.rejected.type]: (state, action: PayloadAction<void, string, never, DomainError>) => {
         state.loadingConferenceDataError = action.error;
      },
   },
});

export const { closeDialog } = createConference.actions;

export default createConference.reducer;
