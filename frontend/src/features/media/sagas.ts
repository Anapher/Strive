import { put, select } from 'redux-saga/effects';
import { takeEverySynchronizedObjectChange } from 'src/store/saga-utils';
import { selectOtherParticipants } from '../conference/selectors';
import { ParticipantDto } from '../conference/types';
import { removeParticipantAudio, setParticipantAudio } from './mediaSlice';
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
   const participants: ParticipantDto[] | null = yield select(selectOtherParticipants);
   const audioInfo: { [id: string]: ParticipantAudioInfo } = yield select(selectParticipantAudio);

   if (!participants) return;

   // add missing participants
   for (const { participantId } of participants.filter((x) => !audioInfo[x.participantId])) {
      yield put(setParticipantAudio({ participantId, data: DEFAULT_PARTICIPANT_AUDIO }));
   }

   // remove participants
   for (const participantId of Object.keys(audioInfo).filter(
      (id) => !participants.find((x) => x.participantId === id),
   )) {
      yield put(removeParticipantAudio(participantId));
   }
}

function* mySaga() {
   yield* takeEverySynchronizedObjectChange('participants', updateParticipants);
}

export default mySaga;
