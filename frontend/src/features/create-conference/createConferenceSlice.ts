import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import * as conferenceServices from 'src/services/api/conference';
import { CreateConferenceDto, CreateConferenceResponse } from './types';

export type CreateConferenceState = {
   dialogOpen: boolean;
   isCreating: boolean;
   createdConferenceId: string | null;
};

const initialState: CreateConferenceState = {
   dialogOpen: false,
   isCreating: false,
   createdConferenceId: null,
};

export const createConferenceAsync = createAsyncThunk('createConference/create', async (dto: CreateConferenceDto) => {
   return await conferenceServices.create(dto);
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
   },
});

export const { openCreateDialog, closeCreateDialog } = createConference.actions;
export default createConference.reducer;
