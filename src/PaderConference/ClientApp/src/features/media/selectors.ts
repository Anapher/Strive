import { ProducerInfo, ProducerSource } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';

const getId = (_: unknown, id: string | undefined) => id;
const getStreams = (state: RootState) => state.media.streams;

export const getParticipantProducers = createSelector(getStreams, getId, (streams, participantId) => {
   if (!streams) return undefined;
   if (!participantId) return undefined;

   const participantStreams = streams[participantId];
   if (!participantStreams) return undefined;

   const result: ParticipantProducerViewModels = {
      mic: undefined,
      webcam: undefined,
      screen: undefined,
   };

   for (const [id, producer] of Object.entries(participantStreams.producers)) {
      if (producer.selected && producer?.kind) {
         result[producer.kind] = { ...producer, id };
      }
   }

   return result;
});

export type ProducerViewModel = ProducerInfo & {
   id: string;
};

export type ParticipantProducerViewModels = { [key in ProducerSource]: ProducerViewModel | undefined };
