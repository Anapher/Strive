import { UseMediaStateInfo } from './store/webrtc/hooks/useMedia';
import { ProducerDevice, ProducerSource } from './store/webrtc/types';

export type EquipmentCommandAction = 'enable' | 'disable' | 'pause' | 'resume' | 'switchDevice';

export type EquipmentCommand = {
   source: ProducerSource;
   deviceId?: string;
   action: EquipmentCommandAction;
};

export type EquipmentStatus = {
   [key in ProducerSource]: UseMediaStateInfo;
};

export type EquipmentDevice = {
   deviceId: string;
   label?: string;
   source: ProducerDevice;
};

export type InitializeEquipmentDto = {
   name: string;
   devices: EquipmentDevice[];
};

export type EquipmentKickedReason = 'participantLeft';
export type RequestDisconnectDto = {
   reason: EquipmentKickedReason;
};
