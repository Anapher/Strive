import { spawn } from 'redux-saga/effects';
import scenesSaga from 'src/features/scenes/sagas';
import mediaSaga from 'src/features/media/sagas';

export default function* rootSaga() {
   yield spawn(scenesSaga);
   yield spawn(mediaSaga);
}
