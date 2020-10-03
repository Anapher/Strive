import { HubConnection } from '@microsoft/signalr';

export type Options = {
   prefix?: string;
   onOpen?: (hub: HubConnection) => void;
   url: string;
   getOptions?: (getState: () => unknown) => IHttpConnectionOptions;
};
