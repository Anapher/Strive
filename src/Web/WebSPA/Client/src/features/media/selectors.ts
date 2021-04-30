import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { ProducerDevice } from 'src/store/webrtc/types';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { selectParticipantsOfCurrentRoom } from '../rooms/selectors';
import { ProducerInfo } from './types';

const getId = (_: unknown, id: string | undefined) => id;
export const selectStreams = (state: RootState) => state.media.synchronized?.streams;

export const selectParticipantAudio = (state: RootState) => state.media.participantAudio;

export const selectParticipantProducers = createSelector(selectStreams, getId, (streams, participantId) => {
   if (!streams) return undefined;
   if (!participantId) return undefined;

   const participantStreams = streams[participantId];
   if (!participantStreams) return undefined;

   return participantStreams.producers;
});

export const selectParticipantsOfRoomWebcamAvailable = createSelector(
   selectParticipantsOfCurrentRoom,
   selectStreams,
   (participants, streams) => {
      if (!streams) return [];

      return Object.entries(streams)
         .filter(([id, media]) => participants.includes(id) && media?.producers.webcam)
         .map(([id]) => id);
   },
);

export const selectParticipantMicActivated = createSelector(selectStreams, getId, (streams, participantId) => {
   if (!streams) return false;
   if (!participantId) return false;

   const participantStreams = streams[participantId];
   if (!participantStreams) return false;

   return participantStreams.producers?.mic?.paused === false;
});

export const selectParticipantAudioInfo = createSelector(selectParticipantAudio, getId, (audios, participantId) => {
   if (!participantId) return undefined;

   return audios[participantId];
});

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

export const selectSpeakingParticipants = createSelector(selectParticipantAudio, (audios) => {
   return Object.entries(audios).filter(([, info]) => info?.speaking);
});

export type ProducerViewModel = ProducerInfo & {
   id: string;
};

export type ParticipantProducerViewModels = { [key in ProducerDevice]?: ProducerViewModel };
