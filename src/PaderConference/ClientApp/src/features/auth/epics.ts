import { AxiosError } from 'axios';
import { RootEpic } from 'pader-conference';
import { from, of } from 'rxjs';
import { catchError, filter, map, switchMap } from 'rxjs/operators';
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
