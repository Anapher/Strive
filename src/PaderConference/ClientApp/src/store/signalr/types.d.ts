import { HubConnection, IHttpConnectionOptions } from '@aspnet/signalr';
import { SIGNALR_CONNECT, SIGNALR_DISCONNECT, SIGNALR_SEND } from './action-types';

export type Options = {
   prefix?: string;
   onOpen?: (hub: HubConnection) => void;
   url: string;
   getOptions?: (getState: () => any) => IHttpConnectionOptions;
};

export type Action =
   | { type: typeof SIGNALR_CONNECT; payload: any }
   | { type: typeof SIGNALR_DISCONNECT; payload: any }
   | { type: typeof SIGNALR_SEND; payload: any };
