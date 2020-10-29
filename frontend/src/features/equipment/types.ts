import { ProducerSource } from '../media/types';

export type EquipmentDeviceInfo = {
   deviceId: string;
   label: string;
   source: ProducerSource;
};

export type RegisterEquipmentRequestDto = {
   name: string;
   devices: EquipmentDeviceInfo[];
};
