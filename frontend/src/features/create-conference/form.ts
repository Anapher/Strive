import { ConferenceType, CreateConferenceDto, Permissions } from './types';

export type UserInfo = {
   name: string;
   id: string;
};

export type CreateConferenceFormState = {
   name?: string | null;
   moderators: UserInfo[];

   conferenceType: ConferenceType;

   startTime?: string | null;
   endTime?: string | null;

   scheduleCron?: string | null;

   permissions?: Permissions | null;
   defaultRoomPermissions?: Permissions | null;
   moderatorPermissions?: Permissions | null;

   enableStartTime: boolean;
   schedule: boolean;
};

export const mapFormToDto = (form: CreateConferenceFormState): CreateConferenceDto => ({
   name: form.name,
   moderators: form.moderators.map((x) => x.id),

   conferenceType: form.conferenceType,

   startTime: form.enableStartTime ? form.startTime : undefined,
   endTime: undefined,

   scheduleCron: form.schedule ? form.scheduleCron : undefined,

   permissions: form.permissions,
   defaultRoomPermissions: form.defaultRoomPermissions,
   moderatorPermissions: form.moderatorPermissions,
});
