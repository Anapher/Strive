import { ProducerInfo } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { ProducerSource } from 'src/store/webrtc/types';
import { ParticipantDto } from '../conference/types';

const getId = (_: unknown, id: string | undefined) => id;
const selectStreams = (state: RootState) => state.media.streams;
const selectParticipants = (state: RootState) => state.conference.participants;

export const selectParticipantProducers = createSelector(selectStreams, getId, (streams, participantId) => {
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

export const selectScreenSharingParticipants = createSelector(
   selectStreams,
   selectParticipants,
   (streams, participants) => {
      if (!streams) return undefined;

      return Object.entries(streams)
         .filter(
            ([, pstreams]) =>
               pstreams && Object.values(pstreams.producers).find((x) => x.kind === 'screen' && !x.paused),
         )
         .map(([participantId]) => participants?.find((x) => x.participantId === participantId))
         .filter((x) => !!x) as ParticipantDto[];
   },
);

export type ProducerViewModel = ProducerInfo & {
   id: string;
};

export type ParticipantProducerViewModels = { [key in ProducerSource]: ProducerViewModel | undefined };
