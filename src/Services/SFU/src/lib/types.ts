import { Producer } from 'mediasoup/lib/types';
import { StreamAction } from './conference/request-types';

export type ConferenceInfoDto = {
   participantToRoom: { [key: string]: string };
   participantPermissions: { [key: string]: { [key: string]: any } };
};

export type ConferenceInfoUpdateDto = ConferenceInfoDto & {
   removedParticipants: string[];
};

export type ConferenceInfo = {
   participantToRoom: Map<string, string>;
   participantPermissions: Map<string, { [key: string]: any }>;
};

export type ConferenceInfoUpdate = ConferenceInfo & {
   removedParticipants: string[];
};

export type ChangeParticipantProducerDto = {
   participantId: string;
   source: ProducerSource;
   action: StreamAction;
};

export const producerSources = [
   'mic',
   'webcam',
   'screen',
   'loopback-mic',
   'loopback-webcam',
   'loopback-screen',
] as const;

export type ProducerSource = typeof producerSources[number];

export type ProducerLink = {
   connectionId: string;
   producer: Producer;
};
