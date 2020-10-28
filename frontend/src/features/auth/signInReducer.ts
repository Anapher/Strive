import { createAsyncThunk, createReducer } from '@reduxjs/toolkit';
import { GuestSignInRequest, SignInRequest } from 'MyModels';
import * as authServices from 'src/services/api/auth';

export type SignInState = {
   isSigningIn: boolean;
   isSigningInGuest: boolean;
   error: string | null;
};

const initialState: SignInState = {
   isSigningIn: false,
   isSigningInGuest: false,
   error: null,
};

export const signInAsync = createAsyncThunk('auth/signIn', async (info: SignInRequest) => {
   const accessInfo = await authServices.signIn(info.userName, info.password);
   return { accessInfo, rememberMe: info.rememberMe };
});

export const signInGuestAsync = createAsyncThunk('auth/guestSignIn', async (info: GuestSignInRequest) => {
   const accessInfo = await authServices.signInGuest(info.displayName);
   return { accessInfo };
});

const signInReducer = createReducer(initialState, {
   [signInAsync.pending.type]: (state) => {
      state.isSigningIn = true;
   },
   [signInAsync.fulfilled.type]: (state) => {
      state.isSigningIn = false;
   },
   [signInAsync.rejected.type]: (state, action) => {
      state.isSigningIn = false;
      state.error = action.payload;
   },

   [signInGuestAsync.pending.type]: (state) => {
      state.isSigningInGuest = true;
   },
   [signInGuestAsync.fulfilled.type]: (state) => {
      state.isSigningInGuest = false;
   },
   [signInGuestAsync.rejected.type]: (state, action) => {
      state.isSigningIn = false;
      state.error = action.payload;
   },
});

export default signInReducer;
