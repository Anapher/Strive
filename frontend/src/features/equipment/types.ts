import { InputDeviceDto } from '../settings/types';

export type RegisterEquipmentRequestDto = {
   name: string;
   devices: InputDeviceDto[];
};
