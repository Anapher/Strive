import { takeEvery } from 'redux-saga/effects';
import { reduxThunkShowError } from 'src/store/notifier/utils';
import { createConferenceAsync, loadUserInfo } from './reducer';

export default function* mySaga() {
   yield takeEvery(loadUserInfo.rejected.type, reduxThunkShowError);
   yield takeEvery(createConferenceAsync.rejected.type, reduxThunkShowError);
}
