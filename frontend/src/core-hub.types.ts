import { UseMediaStateInfo } from './store/webrtc/hooks/useMedia';
import { ProducerSource } from './store/webrtc/types';

export type SendChatMessageDto = {
   message: string;
   mode: SendingMode | null;
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

export type OpenBreakoutRoomsDto = BreakoutRoomsOptions & {
   assignedRooms?: string[][];
};

export type BreakoutRoomsOptions = {
   duration?: string;
   description?: string;
   amount: number;
};

export type PermissionValue = number | string | boolean;

export type Permissions = { [key: string]: PermissionValue };

export type PermissionLayer = {
   order: number;
   name: string;
   permissions: Permissions;
};

export type ParticipantPermissionInfo = {
   participantId: string;
   layers: PermissionLayer[];
};

export type SetTemporaryPermissionDto = {
   participantId: string;
   permissionKey: string;
   value?: PermissionValue;
};
