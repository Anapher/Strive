import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { isProducerDevice, ProducerDevice } from 'src/store/webrtc/types';
import { selectParticipants } from '../conference/selectors';
import { Participant } from '../conference/types';
import { ProducerInfo } from './types';

const getId = (_: unknown, id: string | undefined) => id;
export const selectStreams = (state: RootState) => state.media.streams;

export const selectParticipantAudio = (state: RootState) => state.media.participantAudio;

export const selectParticipantProducers = createSelector(selectStreams, getId, (streams, participantId) => {
   if (!streams) return undefined;
   if (!participantId) return undefined;

   const participantStreams = streams[participantId];
   if (!participantStreams) return undefined;

   const result: ParticipantProducerViewModels = {};

   for (const [id, producer] of Object.entries(participantStreams.producers)) {
      if (producer.selected && producer?.kind && isProducerDevice(producer.kind)) {
         result[producer.kind] = { ...producer, id };
      }
   }

   return result;
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
         .filter(
            ([, pstreams]) =>
               pstreams && Object.values(pstreams.producers).find((x) => x.kind === 'screen' && !x.paused),
         )
         .map(([participantId]) => participants.find((x) => x.id === participantId))
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
