import { Permissions } from 'src/core-hub.types';

export type UserInfo = {
   name: string;
   id: string;
};

// export type CreateConferenceFormState = {
//    permissions: {};
// };

// export const mapFormToDto = (form: CreateConferenceFormState): CreateConferenceDto => ({
//    name: form.name,
//    moderators: form.moderators.map((x) => x.id),

//    conferenceType: form.conferenceType,

//    startTime: form.enableStartTime ? form.startTime : undefined,
//    endTime: undefined,

//    scheduleCron: form.schedule ? form.scheduleCron : undefined,

//    permissions: form.permissions,
//    defaultRoomPermissions: form.defaultRoomPermissions,
//    moderatorPermissions: form.moderatorPermissions,
// });
