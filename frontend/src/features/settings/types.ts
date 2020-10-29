import { ProducerSource } from '../media/types';

export type InputDeviceDto = {
   deviceId: string;
   label?: string;
   source: ProducerSource;
};

export type AnyInputDevice = LocalInputDevice | EquipmentInputDevice;

export type LocalInputDevice = {
   deviceId: string;
   type: 'local';
};

export type EquipmentInputDevice = {
   deviceId: string;
   equipmentId: string;
   type: 'equipment';
};
