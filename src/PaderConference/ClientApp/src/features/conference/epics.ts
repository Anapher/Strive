import { AxiosError } from 'axios';
import { RootEpic } from 'pader-conference';
import { from, of } from 'rxjs';
import { catchError, filter, map, switchMap } from 'rxjs/operators';
import toErrorResult from 'src/utils/error-result';
import { isActionOf } from 'typesafe-actions';
import * as actions from './actions';

export const createEpic: RootEpic = (action$, _, { api }) =>
   action$.pipe(
      filter(isActionOf(actions.createConferenceAsync.request)),
      switchMap(({ payload }) =>
         from(api.conference.create(payload)).pipe(
            map((response) => actions.createConferenceAsync.success(response)),
            catchError((error: AxiosError) => of(actions.createConferenceAsync.failure(toErrorResult(error)))),
         ),
      ),
   );
