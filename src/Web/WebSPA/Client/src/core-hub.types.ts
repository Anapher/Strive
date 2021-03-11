import { UseMediaStateInfo } from './store/webrtc/hooks/useMedia';
import { ProducerSource } from './store/webrtc/types';

export type SyncStatePayload = { id: string; value: any };

export type FetchChatMessagesDto = { channel: string; start: number; end: number };

export type SendChatMessageDto = {
   message: string;
   channel: string;
   options: ChatMessageOptions;
};

export type ChatMessageOptions = {
   isAnnouncement: boolean;
   isAnonymous: boolean;
};

export type ChatMessageDto = {
   id: string;
   channel: string;
   sender?: ChatMessageSender;
   message: string;
   timestamp: string;
   options: ChatMessageOptions;
};

export type ChatMessageSender = {
   participantId: string;
   meta: ParticipantMetadata;
};

export type SetUserIsTypingDto = {
   channel: string;
   isTyping: boolean;
};

export type ParticipantMetadata = {
   displayName: string;
};

export type RoomCreationInfo = {
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

export type KickParticipantRequestDto = {
   participantId: string;
};
