import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AccessInfo, SignInResponse } from 'MyModels';
import * as authServices from 'src/services/api/auth';
import { AppThunk } from 'src/store';
import { signInAsync, signInGuestAsync } from './sign-in-reducer';

export type AuthState = {
   isAuthenticated: boolean;
   rememberMe: boolean;
   token: AccessInfo | null;
};

const initialState: AuthState = {
   isAuthenticated: false,
   rememberMe: false,
   token: null,
};

const auth = createSlice({
   name: 'auth',
   initialState,
   reducers: {
      signOut(state) {
         state.isAuthenticated = false;
         state.token = null;
      },
      refreshTokenSuccess(state, action: PayloadAction<AccessInfo>) {
         state.token = action.payload;
      },
   },
   extraReducers: {
      [signInAsync.fulfilled.type]: (state, action: PayloadAction<SignInResponse>) => {
         state.isAuthenticated = true;
         state.token = action.payload.accessInfo;
         state.rememberMe = action.payload.rememberMe;
      },
      [signInGuestAsync.fulfilled.type]: (state, action: PayloadAction<SignInResponse>) => {
         state.isAuthenticated = true;
         state.token = action.payload.accessInfo;
         state.rememberMe = action.payload.rememberMe;
      },
      [signInAsync.rejected.type]: (state) => {
         state.isAuthenticated = false;
      },
   },
});

export const { refreshTokenSuccess, signOut } = auth.actions;

export const refreshToken = (accessInfo: AccessInfo): AppThunk => async (dispatch) => {
   try {
      const newAccessToken = await authServices.refreshToken(accessInfo);
      dispatch(refreshTokenSuccess(newAccessToken));
   } catch (err) {
      dispatch(signOut);
   }
};

export default auth.reducer;
