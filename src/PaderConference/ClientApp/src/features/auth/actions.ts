import { AccessInfo, SignInRequest } from 'MyModels';
import { IRequestErrorResponse } from 'src/utils/error-result';
import { createAction, createAsyncAction } from 'typesafe-actions';

export const signInAsync = createAsyncAction(
   'AUTH/SIGNIN_REQUEST',
   'AUTH/SIGNIN_SUCCESS',
   'AUTH/SIGNIN_FAILURE',
)<SignInRequest, AccessInfo, IRequestErrorResponse>();

export const refreshTokenAsync = createAsyncAction(
   'AUTH/REFRESH_TOKEN_REQUEST',
   'AUTH/REFRESH_TOKEN_SUCCESS',
   'AUTH/REFRESH_TOKEN_FAILURE',
)<AccessInfo, AccessInfo, IRequestErrorResponse>();

export const signOut = createAction('AUTH/SIGNOUT');
