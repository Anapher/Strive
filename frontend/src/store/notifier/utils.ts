import { PayloadAction } from '@reduxjs/toolkit';
import { ActionPattern, put, takeEvery } from 'redux-saga/effects';
import { SuccessOrError } from 'src/communication-types';
import { showMessage } from './actions';

export function showErrorOn<P extends ActionPattern>(pattern: P) {
   return takeEvery(pattern, function* (action: PayloadAction<SuccessOrError>) {
      if (!action.payload.success) {
         yield put(showMessage({ type: 'error', message: action.payload.error.message }));
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
            yield put(showMessage({ type: 'error', message: payload.error.message }));
         }
      }
   });
}
