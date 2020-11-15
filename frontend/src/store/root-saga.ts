import { spawn } from 'redux-saga/effects';
import scenesSaga from 'src/features/scenes/sagas';

export default function* rootSaga() {
   yield spawn(scenesSaga);
}
