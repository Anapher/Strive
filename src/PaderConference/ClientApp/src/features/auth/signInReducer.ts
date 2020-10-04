import { createAsyncThunk, createReducer } from '@reduxjs/toolkit';
import { SignInRequest } from 'MyModels';
import * as authServices from 'src/services/api/auth';

export type SignInState = {
   isLoading: boolean;
   error: string | null;
};

const initialState: SignInState = {
   isLoading: false,
   error: null,
};

export const signInAsync = createAsyncThunk('auth/signIn', async (info: SignInRequest) => {
   const accessInfo = await authServices.signIn(info.userName, info.password);
   return { accessInfo, rememberMe: info.rememberMe };
});

const signInReducer = createReducer(initialState, {
   [signInAsync.pending.type]: (state) => {
      state.isLoading = true;
   },
   [signInAsync.fulfilled.type]: (state) => {
      state.isLoading = false;
   },
   [signInAsync.rejected.type]: (state, action) => {
      state.isLoading = false;
      state.error = action.payload;
   },
});

export default signInReducer;
