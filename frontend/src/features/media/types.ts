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
