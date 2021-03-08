import { Permissions } from 'src/core-hub.types';

export type ParticipantData = {
   isModerator: boolean;
   displayName: string;
};

export type Participant = ParticipantData & {
   id: string;
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
