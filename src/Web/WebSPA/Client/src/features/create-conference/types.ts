import { Permissions } from 'src/core-hub.types';
import { Scene } from '../scenes/types';

export type CreateConferenceResponse = {
   conferenceId: string;
};

export type ChatOptions = {
   cancelParticipantIsTypingAfter: number;
   showTyping: boolean;
   isRoomChatEnabled: boolean;
   isDefaultRoomChatDisabled: boolean;
   isGlobalChatEnabled: boolean;
   isPrivateChatEnabled: boolean;
};

export type SceneLayoutType = 'chips' | 'chipsWithPresenter' | 'tiles';
export type SceneLayoutTypeWithAuto = 'auto' | SceneLayoutType;
export type SceneOptions = {
   defaultScene: Scene['type'];
   sceneLayout: SceneLayoutTypeWithAuto;
   screenShareLayout: SceneLayoutTypeWithAuto;
   hideParticipantsWithoutWebcam: boolean;
};

export type PermissionType = 'conference' | 'moderator' | 'breakoutRoom';

export type ConferenceConfiguration = {
   name?: string | null;
   moderators: string[];
   startTime?: string | null;
   scheduleCron?: string | null;

   chat: ChatOptions;
   scenes: SceneOptions;
};

export type ConferencePermissions = { [key in PermissionType]?: Permissions };

export type ConferenceData = {
   configuration: ConferenceConfiguration;
   permissions: ConferencePermissions;
};

export type UserInfoFound = {
   id: string;
   notFound: undefined;
   displayName: string;
};

export type UserInfoNotFound = {
   id: string;
   notFound: true;
};

export type UserInfo = UserInfoFound | UserInfoNotFound;
