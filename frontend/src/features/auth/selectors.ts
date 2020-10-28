import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { parseJwt } from 'src/utils/token-helpers';

export const selectAccessToken = createSelector(
   (state: RootState) => state.auth.token?.accessToken,
   (token) => (token ? parseJwt(token) : undefined),
);
