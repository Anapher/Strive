import { Permissions } from 'src/core-hub.types';

export type ParticipantDto = {
   participantId: string;
   role: string;
   displayName?: string;
};

export type ConferenceInfo = {
   conferenceState: 'active' | 'inactive';
   scheduledDate?: string;
   isOpen: boolean;
   permissions: Permissions;
   moderators: string[];
};

export type TemporaryPermissions = { [participantId: string]: Permissions };
