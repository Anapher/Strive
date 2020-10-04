import { RootState } from 'pader-conference';
import { HubConnection } from '@microsoft/signalr';

export type Options = {
   onOpen?: (hub: HubConnection) => void;
   url: string;
   getAccessToken: (state: RootState) => string;
};

export type ParticipantDto = {
   participantId: string;
   role: string;
   displayName?: string;
   attributes: { [key: string]: string };
};
