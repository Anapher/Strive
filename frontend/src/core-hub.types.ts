import { UseMediaStateInfo } from './store/webrtc/hooks/useMedia';
import { ProducerSource } from './store/webrtc/types';

export type SendChatMessageDto = {
   message: string;
   mode?: SendingMode;
};

export type SendingMode = SendAnonymously | SendPrivately;

export type SendAnonymously = {
   type: 'anonymously';
};

export type SendPrivately = {
   type: 'privately';
   to: ParticipantRef;
};

export type ParticipantRef = {
   participantId: string;
   displayName?: string;
};

export type CreateRoomDto = {
   displayName: string;
};

export type SwitchRoomDto = {
   roomId: string;
};

export type EquipmentCommandAction = 'enable' | 'disable' | 'pause' | 'resume' | 'switchDevice';

export type EquipmentCommand = {
   equipmentId: string;
   source: ProducerSource;
   deviceId?: string;
   action: EquipmentCommandAction;
};

export type EquipmentStatus = {
   [key in ProducerSource]: UseMediaStateInfo;
};
