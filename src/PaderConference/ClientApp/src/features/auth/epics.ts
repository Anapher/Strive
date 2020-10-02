import { RootEpic } from 'pader-conference';
import { AxiosError } from 'axios';
import { from, of } from 'rxjs';
import { catchError, filter, map, mapTo, switchMap } from 'rxjs/operators';
import * as signalr from 'src/store/signalr';
import toErrorResult from 'src/utils/error-result';
import { isActionOf } from 'typesafe-actions';
import * as actions from './actions';

export const signInEpic: RootEpic = (action$, _, { api }) =>
   action$.pipe(
      filter(isActionOf(actions.signInAsync.request)),
      switchMap(({ payload: { userName, password } }) =>
         from(api.auth.signIn(userName, password)).pipe(
            map((response) => actions.signInAsync.success(response)),
            catchError((error: AxiosError) => of(actions.signInAsync.failure(toErrorResult(error)))),
         ),
      ),
   );

export const refreshTokenEpic: RootEpic = (action$, _, { api }) =>
   action$.pipe(
      filter(isActionOf(actions.refreshTokenAsync.request)),
      switchMap(({ payload }) =>
         from(api.auth.refreshToken(payload)).pipe(
            map((response) => actions.refreshTokenAsync.success(response)),
            catchError(() => {
               return of(actions.signOut());
            }),
         ),
      ),
   );

// its very important that SignalR disconnected on sign out, because when a different user signs in,
// it might still run with the auth token from the previous user -> very bad
export const signOutEpic: RootEpic = (action$) =>
   action$.pipe(filter(isActionOf(actions.signOut)), mapTo(signalr.disconnect()) as any);
