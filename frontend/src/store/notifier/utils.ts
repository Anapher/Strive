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
