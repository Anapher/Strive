import { ProducerDevice } from 'src/store/webrtc/types';

export type InputDeviceDto = {
   deviceId: string;
   label?: string;
   source: ProducerDevice;
};

export type AnyInputDevice = LocalInputDevice | EquipmentInputDevice;

export type LocalInputDevice = {
   deviceId: string;
   type: 'local';
};

export type EquipmentInputDevice = {
   deviceId: string;
   connectionId: string;
   type: 'equipment';
};
