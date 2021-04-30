import { Permissions } from 'src/core-hub.types';
import { ParticipantData } from 'src/features/conference/types';
import { SceneOptions } from 'src/features/create-conference/types';
import { EquipmentConnection } from 'src/features/media/types';

export const ROOMS = 'rooms';
export const CONFERENCE = 'conference';
export const PARTICIPANTS = 'participants';
export const PARTICIPANT_PERMISSIONS = 'participantPermissions';
export const CHAT = 'chat';
export const SUBSCRIPTIONS = 'subscriptions';
export const BREAKOUT_ROOMS = 'breakoutRooms';
export const MEDIA = 'media';
export const EQUIPMENT = 'equipment';
export const SCENE = 'scene';
export const SCENE_TALKINGSTICK = 'scene_talkingStick';
export const TEMPORARY_PERMISSIONS = 'temporaryPermissions';

export type SynchronizedParticipants = {
   participants: { [participantId: string]: ParticipantData };
};

export type SynchronizedConferenceInfo = {
   isOpen: boolean;
   moderators: string[];
   scheduledDate?: string | null;
   name: string | null;
   isPrivateChatEnabled: boolean;
   sceneOptions: SceneOptions;
};

export type SynchronizedParticipantsPermissions = {
   permissions: Permissions;
};

export type SynchronizedRooms = {
   rooms: Room[];
   defaultRoomId: string;
   participants: { [participanId: string]: string };
};

export type Room = {
   roomId: string;
   displayName: string;
};

export type ChatSynchronizedObject = {
   participantsTyping: { [id: string]: boolean };
};

export type SynchronizedEquipment = {
   connections: Record<string, EquipmentConnection>;
};
