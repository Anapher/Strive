import _ from 'lodash';
import { DateTime } from 'luxon';
import { cancel, delay, fork, put, select, take, takeEvery } from 'redux-saga/effects';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { ParticipantDto } from '../conference/types';
import { patchParticipantAudio, removeParticipantAudio, setParticipantAudio } from '../media/mediaSlice';
import { selectParticipantAudio, selectScreenSharingParticipants } from '../media/selectors';
import { ParticipantAudioInfo } from '../media/types';
import {
   addActiveParticipant,
   removeActiveParticipant,
   setAppliedScene,
   setCurrentScene,
   updateActiveParticipantDeleted,
} from './scenesSlice';
import {
   selectActiveParticipants,
   selectAppliedScene,
   selectCurrentScene,
   selectServerProvidedScene,
} from './selectors';
import { ActiveParticipants, RoomSceneState, Scene, ViewableScene } from './types';
import { applyPatch, generateActiveParticipantsPatch } from './utils';

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
         // order to make it deterministic
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

let orderNumberCounter = 1;

function* updateActiveParticipants(): any {
   const participantAudio: { [id: string]: ParticipantAudioInfo | undefined } = yield select(selectParticipantAudio);
   const currentScene: ViewableScene = yield select(selectCurrentScene);
   const activeParticipants: ActiveParticipants = yield select(selectActiveParticipants);

   const currentlySpeaking = Object.entries(participantAudio)
      .filter(([, audio]) => audio?.speaking)
      .map(([id]) => id);

   const presentators = getPresentors(currentScene);

   const currentActiveParticipants = [...presentators, ...currentlySpeaking];

   const update = generateActiveParticipantsPatch(activeParticipants, currentActiveParticipants);

   // immediately
   for (const participantId of update.newParticipants) {
      yield put(addActiveParticipant({ participantId, orderNumber: orderNumberCounter++ }));
   }

   for (const participantId in update.updatedParticipants) {
      yield put(
         updateActiveParticipantDeleted({
            participantId,
            deletedOn: update.updatedParticipants[participantId].deletedOn,
         }),
      );
   }

   for (const participantId of update.removedParticipants) {
      yield put(removeActiveParticipant(participantId));
   }

   const newActiveParticipants = applyPatch(update, activeParticipants);
   const updateRequiredTime = _(Object.values(newActiveParticipants))
      .filter((x) => !!x.deletedOn)
      .map((x) => DateTime.fromISO(x.deletedOn!))
      .orderBy((x) => x, 'asc')
      .first();

   if (updateRequiredTime) {
      const diff = updateRequiredTime.diffNow('milliseconds');
      yield delay(diff.milliseconds);
      yield fork(updateActiveParticipants);
   }
}

function getPresentors(scene: ViewableScene): string[] {
   switch (scene.type) {
      case 'screenshare':
         return [scene.participantId];
      default:
         return [];
   }
}

function* main() {
   let currentTask: any | undefined;
   while (
      yield take([
         removeParticipantAudio.type,
         setParticipantAudio.type,
         patchParticipantAudio.type,
         setCurrentScene.type,
         setAppliedScene.type,
      ])
   ) {
      if (currentTask) yield cancel(currentTask);

      // starts the task in the background
      currentTask = yield fork(updateActiveParticipants);
   }
}

function* mySaga() {
   yield* takeEverySynchronizedObjectChange('scenes', updateScene);
   yield* takeEverySynchronizedObjectChange('rooms', updateScene);

   yield takeEvery(setAppliedScene.type, updateAutomaticScene);
   yield* takeEverySynchronizedObjectChange('mediaStreams', updateAutomaticScene);

   yield fork(main);
}

export default mySaga;
