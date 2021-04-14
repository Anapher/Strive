import { DomainError } from './communication-types';

export const unknownRequestError: (info: string) => DomainError = (info) => ({
   code: 'UI/Request_Error',
   message: `An internal error occurred when executing a request: ${info}`,
   type: 'InternalServerError',
   fields: {
      info,
   },
});

export const serverUnavailable: () => DomainError = () => ({
   code: 'UI/Server_Unavailable',
   message: `The server is currently unavailable. Please check your internet connection.`,
   type: 'BadRequest',
});

export const signalRConnectionUnavailable: (message: string) => DomainError = (message) => ({
   code: 'SignalR/Connection_Error',
   message,
   type: 'InternalServerError',
});

export const equipmentError: (message: string) => DomainError = (message) => ({
   code: 'UI/Equipment_Error',
   message,
   type: 'InternalServerError',
});

export const newSessionConnectedError: () => DomainError = () => ({
   code: 'UI/New_Session_Connected',
   message: 'You connected from a different session to this conference.',
   type: 'Conflict',
});

export const kickedError: () => DomainError = () => ({
   code: 'UI/Kicked',
   message: 'You were kicked from this conference.',
   type: 'Conflict',
});
