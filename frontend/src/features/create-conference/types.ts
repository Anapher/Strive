export type ConferenceType = 'class' | 'presentation';

export type PermissionValue = number | string | boolean;

export type Permissions = { [key: string]: PermissionValue };

export type CreateConferenceDto = {
   name?: string | null;
   moderators: string[];

   conferenceType: ConferenceType;

   startTime?: string | null;
   endTime?: string | null;

   scheduleCron?: string | null;
   permissions?: Permissions | null;
   defaultRoomPermissions?: Permissions | null;
   moderatorPermissions?: Permissions | null;
};

export type CreateConferenceResponse = {
   conferenceId: string;
};
