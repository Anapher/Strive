import { ConsumerScore, ConsumerType, MediaKind, RtpParameters } from 'mediasoup/lib/types';
import { ProducerSource } from '../types';
import { ConnectionMessage } from './sub-types';

export type ProducerChangedEventArgs = {
   source: ProducerSource;
   action: 'pause' | 'resume' | 'close';
   producerId: string;
};

export type ConsumerArgs = {
   consumerId: string;
};

export type ConsumerCreatedArgs = {
   participantId: string;
   producerId: string;
   id: string;
   kind: MediaKind;
   rtpParameters: RtpParameters;
   type: ConsumerType;
   appData: any;
   producerPaused: boolean;
};

export type ConsumerScoreArgs = ConsumerArgs & { score: ConsumerScore };
export type ConsumerLayersChanged = ConsumerArgs & {
   spatialLayer?: number | null;
   temporalLayer?: number | null;
};

export type ProducerScoreInfo = {
   producerId: string;
   score: number;
};

export type ConsumerInfo = {
   paused: boolean;
   participantId: string;
   loopback?: boolean;
};

export type ProducerInfo = {
   paused: boolean;
   selected: boolean;
   kind?: ProducerSource;
};

export type ParticipantStreams = {
   consumers: {
      [key: string]: ConsumerInfo;
   };
   producers: { [key: string]: ProducerInfo };
};

export type ConferenceParticipantStreamInfo = { [key: string]: ParticipantStreams };
