import { PayloadAction } from '@reduxjs/toolkit';
import { ActionPattern, put, takeEvery } from 'redux-saga/effects';
import { DomainError, SuccessOrError } from 'src/communication-types';
import { formatErrorMessage } from 'src/utils/error-utils';
import { showMessage } from './actions';

export function showErrorOn<P extends ActionPattern>(pattern: P) {
   return takeEvery(pattern, function* (action: PayloadAction<SuccessOrError>) {
      if (!action.payload.success) {
         yield put(showMessage({ type: 'error', message: formatErrorMessage(action.payload.error) }));
      }
   });
}

type CoreHubAction = {
   action: string;
   returnAction: string;
};

export function showLoadingHubAction(hubAction: CoreHubAction, message: string) {
   return takeEvery([hubAction.action, hubAction.returnAction], function* (action: PayloadAction<any>) {
      if (action.type === hubAction.action) {
         yield put(showMessage({ type: 'loading', message, dismissOn: { type: hubAction.returnAction } }));
      } else {
         const payload = action.payload as SuccessOrError;
         if (!payload.success) {
            yield put(showMessage({ type: 'error', message: formatErrorMessage(payload.error) }));
         }
      }
   });
}

export function* reduxThunkShowError({ error }: PayloadAction<void, string, never, DomainError>) {
   yield put(showMessage({ type: 'error', message: formatErrorMessage(error) }));
}
