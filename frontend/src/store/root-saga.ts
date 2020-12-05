import { spawn } from 'redux-saga/effects';
import scenesSaga from 'src/features/scenes/sagas';
import mediaSaga from 'src/features/media/sagas';
import settingsSaga from 'src/features/settings/sagas';

export default function* rootSaga() {
   yield spawn(scenesSaga);
   yield spawn(mediaSaga);
   yield spawn(settingsSaga);
}
