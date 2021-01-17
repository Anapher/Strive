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

export type ConferenceLink = {
   conferenceId: string;
   isActive: boolean;
   conferenceName?: string;
   starred: string;
   isModerator: boolean;
   lastJoin: string;
   scheduled?: string;
};

export type TemporaryPermissions = { [participantId: string]: Permissions };
