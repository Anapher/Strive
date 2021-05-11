import { MotionValue } from 'framer-motion';
import { Harker } from 'hark';
import { EquipmentDevice } from 'src/equipment-hub.types';
import { UseMediaStateInfo } from 'src/store/webrtc/hooks/useMedia';
import { ProducerSource } from 'src/store/webrtc/types';

export type ConsumerInfo = {
   paused: boolean;
   participantId: string;
   source: ProducerSource;
};

export type ProducerInfo = {
   paused: boolean;
};

export type ParticipantStreams = {
   consumers: Record<string, ConsumerInfo>;
   producers: Record<ProducerSource, ProducerInfo>;
};

export type SynchronizedMediaState = { streams: Record<string, ParticipantStreams | undefined> };

export type EquipmentConnection = {
   connectionId: string;
   name: string;
   devices: EquipmentDevice[];
   status: Record<ProducerSource, UseMediaStateInfo>;
};

export type ParticipantAudioInfo = {
   volume: number;
   muted: boolean;
   registered: boolean;
   speaking: boolean;
};

export type ParticipantAudioElement = {
   elem: HTMLAudioElement;
   stream: MediaStream;
   audioLevel: MotionValue<number>;
   hark: Harker;
};
