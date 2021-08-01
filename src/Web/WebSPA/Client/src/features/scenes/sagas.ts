import _ from 'lodash';
import { DateTime } from 'luxon';
import { cancel, delay, fork, put, select, take } from 'redux-saga/effects';
import { selectParticipantsMap } from '../conference/selectors';
import { ParticipantData } from '../conference/types';
import { patchParticipantAudio, removeParticipantAudio, setParticipantAudio } from '../media/reducer';
import { selectParticipantAudio } from '../media/selectors';
import { ParticipantAudioInfo } from '../media/types';
import { setActiveParticipants } from './reducer';
import { selectActiveParticipants } from './selectors';
import { ActiveParticipantData, ActiveParticipants } from './types';

const PARTICIPANT_REMOVAL_SLIDING_SECONDS = 5;

const getCurrentySpeakingParticipants = (data: { [id: string]: ParticipantAudioInfo | undefined }) =>
   Object.entries(data)
      .filter(([, audio]) => audio?.speaking)
      .map(([id]) => id);

const mapParticipantEntry = (
   isSpeaking: boolean,
   lastActivity: string | undefined,
   isInactive: boolean,
   now: DateTime,
): Partial<ActiveParticipantData> => {
   const isMarkedAsDeleted = Boolean(lastActivity);
   if (isSpeaking) {
      return {};
   }

   if (isInactive) {
      return { inactive: true };
   }

   if (!isMarkedAsDeleted) {
      return { lastActivity: now.toISO() };
   }

   const deleteOnTimestamp = DateTime.fromISO(lastActivity!).plus({ seconds: PARTICIPANT_REMOVAL_SLIDING_SECONDS });
   if (now >= deleteOnTimestamp) {
      return { inactive: true };
   } else {
      return { lastActivity: lastActivity };
   }
};

function* updateActiveParticipants(): any {
   const participantAudio: { [id: string]: ParticipantAudioInfo | undefined } = yield select(selectParticipantAudio);
   let activeParticipants: ActiveParticipants = yield select(selectActiveParticipants);

   // remove participants that left the conference
   const allParticipants: { [id: string]: ParticipantData } = yield select(selectParticipantsMap);
   activeParticipants = Object.fromEntries(Object.entries(activeParticipants).filter(([id]) => allParticipants[id]));

   // sort participants
   const currentlySpeaking = getCurrentySpeakingParticipants(participantAudio);

   const now = DateTime.utc();
   const updatedActiveParticipants: [string, Partial<ActiveParticipantData>][] = Object.entries(activeParticipants).map(
      ([participantId, state]) => {
         const isSpeaking = currentlySpeaking.includes(participantId);
         const isInactive = Boolean(state.inactive);

         return [participantId, mapParticipantEntry(isSpeaking, state.lastActivity, isInactive, now)];
      },
   );

   for (const participantId of currentlySpeaking.filter((id) => !activeParticipants[id])) {
      updatedActiveParticipants.push([participantId, {}]);
   }

   // we want a list where first we have the active participants, then the inactive ones
   // and we want to preserve the order. Lastly, we want it to be deterministic by sorting by the id

   const orderedList = _.orderBy(
      updatedActiveParticipants,
      [([, state]) => Boolean(state.inactive), ([id]) => activeParticipants[id]?.orderNumber, ([id]) => id],
      ['asc', 'asc', 'asc'],
   );

   const newActiveParticipants = Object.fromEntries(
      orderedList.map(([id, state], i) => [id, { ...state, orderNumber: i }]),
   );
   yield put(setActiveParticipants(newActiveParticipants));

   const updateRequiredTime = _(Object.values(newActiveParticipants))
      .filter((x): x is Required<ActiveParticipantData> => !!x.lastActivity)
      .map((x) => DateTime.fromISO(x.lastActivity))
      .orderBy((x) => x, 'asc')
      .first();

   if (updateRequiredTime) {
      const diff = updateRequiredTime.plus({ seconds: PARTICIPANT_REMOVAL_SLIDING_SECONDS }).diffNow('milliseconds');

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
