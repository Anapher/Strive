import _ from 'lodash';
import { DateTime } from 'luxon';
import { cancel, delay, fork, put, select, take } from 'redux-saga/effects';
import { patchParticipantAudio, removeParticipantAudio, setParticipantAudio } from '../media/reducer';
import { selectParticipantAudio } from '../media/selectors';
import { ParticipantAudioInfo } from '../media/types';
import { addActiveParticipant, removeActiveParticipant, updateActiveParticipantDeleted } from './reducer';
import { selectActiveParticipants } from './selectors';
import { ActiveParticipantData, ActiveParticipants } from './types';
import { applyPatch, generateActiveParticipantsPatch } from './utils';

let orderNumberCounter = 1;

function* updateActiveParticipants(): any {
   const participantAudio: { [id: string]: ParticipantAudioInfo | undefined } = yield select(selectParticipantAudio);
   const activeParticipants: ActiveParticipants = yield select(selectActiveParticipants);

   const currentlySpeaking = Object.entries(participantAudio)
      .filter(([, audio]) => audio?.speaking)
      .map(([id]) => id);

   const update = generateActiveParticipantsPatch(activeParticipants, currentlySpeaking);

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

function* main(): any {
   let currentTask: any | undefined;
   while (yield take([removeParticipantAudio.type, setParticipantAudio.type, patchParticipantAudio.type])) {
      if (currentTask) yield cancel(currentTask);

      // starts the task in the background
      currentTask = yield fork(updateActiveParticipants);
   }
}

function* mySaga() {
   yield fork(main);
}

export default mySaga;
