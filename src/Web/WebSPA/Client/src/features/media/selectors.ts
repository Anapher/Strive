import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { ProducerDevice } from 'src/store/webrtc/types';
import { createArrayEqualSelector } from 'src/utils/reselect';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectParticipantsOfCurrentRoom } from '../rooms/selectors';
import { ProducerInfo } from './types';

export const selectStreams = (state: RootState) => state.media.synchronized?.streams;
export const selectParticipantAudio = (state: RootState) => state.media.participantAudio;

export const selectParticipantProducers = (state: RootState, participantId: string | undefined) => {
   const streams = selectStreams(state);
   if (!streams) return undefined;
   if (!participantId) return undefined;

   const participantStreams = streams[participantId];
   if (!participantStreams) return undefined;

   return participantStreams.producers;
};

// we wrap the selector with a deep equal selector to ensure that if the list stays the same,
// the selector does not update
export const selectParticipantsOfRoomWebcamAvailable = createArrayEqualSelector(
   createSelector(selectParticipantsOfCurrentRoom, selectStreams, (participants, streams) => {
      if (!streams) return [];

      return Object.entries(streams)
         .filter(([id, media]) => participants.includes(id) && media?.producers.webcam)
         .map(([id]) => id);
   }),
   (x) => x,
);

export const selectParticipantMicActivated = (state: RootState, participantId: string | undefined) => {
   const streams = selectStreams(state);

   if (!streams) return false;
   if (!participantId) return false;

   const participantStreams = streams[participantId];
   if (!participantStreams) return false;

   return participantStreams.producers?.mic?.paused === false;
};

export const selectParticipantAudioInfo = (state: RootState, participantId: string | undefined) => {
   if (!participantId) return undefined;
   const audios = selectParticipantAudio(state);

   return audios[participantId];
};

export const selectScreenSharingParticipants = createSelector(
   selectStreams,
   selectParticipants,
   (streams, participants) => {
      if (!streams) return [];

      return Object.entries(streams)
         .filter(([, pstreams]) => pstreams?.producers.screen?.paused === false)
         .map(([participantId]) => participants[participantId])
         .filter((x): x is Participant => !!x);
   },
);

export const selectSpeakingParticipants = createArrayEqualSelector(
   createSelector(selectParticipantAudio, (audios) => {
      return Object.entries(audios).filter(([, info]) => info?.speaking);
   }),
   (x) => x,
);

export type ProducerViewModel = ProducerInfo & {
   id: string;
};

export type ParticipantProducerViewModels = { [key in ProducerDevice]?: ProducerViewModel };
