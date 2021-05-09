import { DomainError } from 'src/communication-types';
import { EquipmentCommand } from 'src/equipment-hub.types';
import { InputDeviceDto } from '../settings/types';

export type RegisterEquipmentRequestDto = {
   name: string;
   devices: InputDeviceDto[];
};

export type EquipmentCommandResult = {
   id: number;
   command: EquipmentCommand;
   error?: DomainError;
   executed?: boolean;
};
