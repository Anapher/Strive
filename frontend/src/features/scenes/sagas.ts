import _ from 'lodash';
import { put, select, takeEvery } from 'redux-saga/effects';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { ParticipantDto } from '../conference/types';
import { selectScreenSharingParticipants } from '../media/selectors';
import { setAppliedScene, setCurrentScene } from './scenesSlice';
import { selectAppliedScene, selectCurrentScene, selectServerProvidedScene } from './selectors';
import { RoomSceneState, Scene, ViewableScene } from './types';

function* updateScene() {
   const scene: RoomSceneState | undefined = yield select(selectServerProvidedScene);
   if (!scene) return;

   if (scene.isControlled) {
      const appliedScene: Scene = yield select(selectAppliedScene);

      if (appliedScene.type !== scene.scene.type) {
         yield put(setAppliedScene(scene.scene));
      }
   }
}

/**
 * If automatic sceen selection is enabled, react to changes and update current scene
 */
function* updateAutomaticScene() {
   const appliedScene: Scene = yield select(selectAppliedScene);

   if (appliedScene.type === 'automatic') {
      const participants: ParticipantDto[] = yield select(selectScreenSharingParticipants);
      const currentScene: ViewableScene = yield select(selectCurrentScene);

      if (participants.length > 0) {
         // to make it deterministic
         const displayedScreenshare = _(participants)
            .orderBy((x) => x.participantId)
            .first();

         if (
            currentScene.type !== 'screenshare' ||
            currentScene.participantId !== displayedScreenshare!.participantId
         ) {
            yield put(setCurrentScene({ type: 'screenshare', participantId: displayedScreenshare!.participantId }));
         }
      } else {
         if (currentScene.type !== 'grid') {
            yield put(setCurrentScene({ type: 'grid' }));
         }
      }
   }
}

function* mySaga() {
   yield* takeEverySynchronizedObjectChange('scenes', updateScene);
   yield* takeEverySynchronizedObjectChange('rooms', updateScene);

   yield takeEvery(setAppliedScene.type, updateAutomaticScene);
   yield* takeEverySynchronizedObjectChange('mediaStreams', updateAutomaticScene);
}

export default mySaga;
