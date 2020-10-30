import { ProducerInfo } from './types';
import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { ProducerSource } from 'src/store/webrtc/types';
import { selectAccessToken } from '../auth/selectors';

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

// const getSource = (_: unknown, source: ProducerSource) => source;

// const defaultMediaStatus: MediaStatus = { enabled: false, paused: false };

// export const selectDeviceState = createSelector(getStreams, selectAccessToken, getSource, (streams, token, source) => {
//    if (!streams) return defaultMediaStatus;
//    if (!token) return defaultMediaStatus;

//    const participantStreams = streams[token.nameid];
//    if (!participantStreams) return defaultMediaStatus;

//    const producer = Object.entries(participantStreams.producers).find((x) => x[1].selected && x[1].kind === source);
//    if (!producer) return defaultMediaStatus;
//    return { enabled: true, paused: producer[1].paused };
// });

export type ProducerViewModel = ProducerInfo & {
   id: string;
};

export type ParticipantProducerViewModels = { [key in ProducerSource]: ProducerViewModel | undefined };
