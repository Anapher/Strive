import { MotionValue } from 'framer-motion';
import { Harker } from 'hark';
import { UseMediaStateInfo } from 'src/store/webrtc/hooks/useMedia';
import { ProducerSource } from 'src/store/webrtc/types';
import { InputDeviceDto } from '../settings/types';

export type ConsumerInfo = {
   paused: boolean;
   participantId: string;
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

export type ConferenceParticipantStreamInfo = { [key: string]: ParticipantStreams | undefined };

export type ConnectedEquipmentDto = {
   equipmentId: string;
   name?: string;
   devices?: InputDeviceDto[];

   status?: { [key in ProducerSource]: UseMediaStateInfo };
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
