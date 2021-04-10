import { PayloadAction } from '@reduxjs/toolkit';
import { put, takeEvery } from 'redux-saga/effects';
import * as coreHub from 'src/core-hub';
import { closeConference, openConference } from 'src/core-hub';
import { RequestDisconnectDto } from 'src/core-hub.types';
import { connectionRequestExecutedError } from 'src/errors';
import { showErrorOn, showLoadingHubAction } from 'src/store/notifier/utils';
import { close, onEventOccurred } from 'src/store/signal/actions';
import { setConnectionError } from './reducer';

function* onRequestDisconnect(action: PayloadAction<RequestDisconnectDto>) {
   yield put(close());
   yield put(setConnectionError(connectionRequestExecutedError(action.payload.reason)));
}

export default function* mySaga() {
   yield showErrorOn(openConference.returnAction);
   yield showErrorOn(closeConference.returnAction);
   yield showLoadingHubAction(coreHub.fetchPermissions, 'Fetch permissions...');

   yield takeEvery(onEventOccurred(coreHub.events.onRequestDisconnect), onRequestDisconnect);
}
