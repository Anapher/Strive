import _ from 'lodash';
import { DateTime } from 'luxon';
import { cancel, delay, fork, put, select, take, takeEvery } from 'redux-saga/effects';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { SCENE } from 'src/store/signal/synchronization/synchronized-object-ids';
import { patchParticipantAudio, removeParticipantAudio, setParticipantAudio } from '../media/reducer';
import { selectParticipantAudio } from '../media/selectors';
import { ParticipantAudioInfo } from '../media/types';
import {
   addActiveParticipant,
   removeActiveParticipant,
   setAppliedScene,
   setCurrentScene,
   updateActiveParticipantDeleted,
} from './reducer';
import { selectActiveParticipants, selectAppliedScene, selectAvailableScenes, selectCurrentScene } from './selectors';
import { ActiveParticipantData, ActiveParticipants, Scene, ViewableScene } from './types';
import { applyPatch, generateActiveParticipantsPatch } from './utils';

function* updateScene() {
   const appliedScene: Scene = yield select(selectAppliedScene);
   const availableScenes: Scene[] = yield select(selectAvailableScenes);
   const currentScene: ViewableScene = yield select(selectCurrentScene);

   const viewableScene = translateScene(appliedScene, availableScenes, currentScene);
   if (_.isEqual(viewableScene, currentScene)) return;

   yield put(setCurrentScene(viewableScene));
}

function translateScene(scene: Scene, availableScenes: Scene[], currentScene: ViewableScene): ViewableScene {
   if (scene.type === 'autonomous') {
      const prefferedScene = getPrefferedScene(availableScenes);
      if (currentScene.type === prefferedScene.type) return currentScene;

      return prefferedScene;
   }

   return scene;
}

const sceneOrder: Scene['type'][] = ['screenShare', 'grid'];

function getPrefferedScene(availableScenes: Scene[]): ViewableScene {
   return _.orderBy(availableScenes, (x) =>
      sceneOrder.indexOf(x.type) === -1 ? Number.MAX_VALUE : sceneOrder.indexOf(x.type),
   )[0] as ViewableScene;
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
      .filter((x): x is Required<ActiveParticipantData> => !!x.deletedOn)
      .map((x) => DateTime.fromISO(x.deletedOn))
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
      case 'screenShare':
         return [scene.participantId];
      default:
         return [];
   }
}

function* main(): any {
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
   yield* takeEverySynchronizedObjectChange(SCENE, updateScene);
   yield takeEvery(setAppliedScene.type, updateScene);
   yield fork(main);
}

export default mySaga;
