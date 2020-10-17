export type ConferenceType = 'class' | 'presentation';

export type Permissions = { [key: string]: string };

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
