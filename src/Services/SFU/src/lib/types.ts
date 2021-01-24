import { Producer } from 'mediasoup/lib/types';

export type ConferenceInfo = {
   participantToRoom: Map<string, string>;
   participantPermissions: Map<string, { [key: string]: any }>;
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
