import { put, select } from 'redux-saga/effects';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { selectMyParticipantId } from '../auth/selectors';
import { selectParticipantList } from '../conference/selectors';
import { Participant } from '../conference/types';
import { removeParticipantAudio, setParticipantAudio } from './reducer';
import { selectParticipantAudio } from './selectors';
import { ParticipantAudioInfo } from './types';

export const DEFAULT_PARTICIPANT_AUDIO: ParticipantAudioInfo = {
   muted: false,
   registered: false,
   speaking: false,
   volume: 0.75,
};

/**
 * synchronize participants list with audio info
 */
function* updateParticipants() {
   const participants: Participant[] = yield select(selectParticipantList);
   const audioInfo: { [id: string]: ParticipantAudioInfo } = yield select(selectParticipantAudio);
   const myId: string = yield select(selectMyParticipantId);

   if (participants.length === 0) return;

   // add missing participants
   for (const { id } of participants.filter((x) => !audioInfo[x.id])) {
      yield put(setParticipantAudio({ participantId: id, data: { ...DEFAULT_PARTICIPANT_AUDIO, muted: id === myId } }));
   }

   // remove participants
   for (const participantId of Object.keys(audioInfo).filter((id) => !participants.find((x) => x.id === id))) {
      yield put(removeParticipantAudio(participantId));
   }
}

function* mySaga() {
   yield* takeEverySynchronizedObjectChange('participants', updateParticipants);
}

export default mySaga;
