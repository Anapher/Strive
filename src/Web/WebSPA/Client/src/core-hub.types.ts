import { EquipmentCommandAction } from './equipment-hub.types';
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

export type OpenBreakoutRoomsDto = BreakoutRoomsConfig & {
   assignedRooms?: string[][];
};

export type BreakoutRoomsConfig = {
   deadline?: string;
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

export type SfuConnectionInfo = {
   url: string;
   authToken: string;
};

export type SendEquipmentCommandDto = {
   connectionId: string;
   source: ProducerSource;
   deviceId?: string;
   action: EquipmentCommandAction;
};
