import { Permissions } from 'src/core-hub.types';

export type ConferenceType = 'class' | 'presentation';

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
