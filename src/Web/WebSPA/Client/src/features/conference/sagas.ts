import { put, takeEvery } from 'redux-saga/effects';
import * as coreHub from 'src/core-hub';
import { closeConference, openConference } from 'src/core-hub';
import { showErrorOn, showLoadingHubAction } from 'src/store/notifier/utils';
import { close, onEventOccurred } from 'src/store/signal/actions';

function* onRequestDisconnect() {
   yield put(close());
}

export default function* mySaga() {
   yield showErrorOn(openConference.returnAction);
   yield showErrorOn(closeConference.returnAction);
   yield showLoadingHubAction(coreHub.fetchPermissions, 'Fetch permissions...');

   yield takeEvery(onEventOccurred(coreHub.events.onRequestDisconnect), onRequestDisconnect);
}
