import { Permissions } from 'src/core-hub.types';

export type CreateConferenceResponse = {
   conferenceId: string;
};

export type ChatOptions = {
   maxChatMessageHistory: number;
   cancelParticipantIsTypingAfter: number;
   cancelParticipantIsTypingInterval: number;
   showTyping: boolean;
};

export type PermissionType = 'conference' | 'moderator' | 'breakoutRoom';

export type ConferenceConfiguration = {
   name?: string | null;
   moderators: string[];
   startTime?: string | null;
   scheduleCron?: string | null;

   chat: ChatOptions;
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
