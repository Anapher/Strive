import { ConferenceType } from '../create-conference/types';

export type ParticipantDto = {
   participantId: string;
   role: string;
   displayName?: string;
};

export type ConferenceInfo = {
   conferenceState: 'active' | 'inactive';
   scheduledDate?: string;
   isOpen: boolean;
   conferenceType?: ConferenceType;
   permissions: Permissions;
   moderators: string[];
};
