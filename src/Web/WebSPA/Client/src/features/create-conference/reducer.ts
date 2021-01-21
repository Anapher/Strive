import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError } from 'src/communication-types';
import * as conferenceServices from 'src/services/api/conference';
import * as userServices from 'src/services/api/user';
import { RootState } from 'src/store';
import { serializeRequestError } from 'src/utils/error-result';
import { selectMyParticipantId } from '../auth/selectors';
import { ConferenceData, CreateConferenceResponse, UserInfo } from './types';

export type CreateConferenceState = {
   loadingConferenceDataError: DomainError | null;
   conferenceData: ConferenceData | null;

   dialogOpen: boolean;
   mode: 'create' | 'edit';

   isCreating: boolean;
   createdConferenceId: string | null;
   userInfo: UserInfo[];
};

const initialState: CreateConferenceState = {
   mode: 'create',
   loadingConferenceDataError: null,
   dialogOpen: false,
   isCreating: false,
   createdConferenceId: null,
   conferenceData: null,
   userInfo: [],
};

export const createConferenceAsync = createAsyncThunk(
   'createConference/create',
   async (dto: ConferenceData) => {
      return await conferenceServices.create(dto);
   },
   {
      serializeError: serializeRequestError,
   },
);

export const loadUserInfo = createAsyncThunk(
   'createConference/loadUserInfo',
   async (ids: string[]) => {
      return await userServices.getUserInfo(ids);
   },
   {
      serializeError: serializeRequestError,
   },
);

export const openDialogToCreateAsync = createAsyncThunk('createConference/openForCreate', async (_, { getState }) => {
   const defaultState = await conferenceServices.getDefault();
   const myId = selectMyParticipantId(getState() as RootState);
   const result: ConferenceData = {
      ...defaultState,
      configuration: { ...defaultState.configuration, moderators: myId ? [myId] : [] },
   };

   return result;
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
      [loadUserInfo.fulfilled.type]: (state, action: PayloadAction<UserInfo[]>) => {
         state.userInfo = action.payload;
      },
   },
});

export const { closeDialog } = createConference.actions;

export default createConference.reducer;
