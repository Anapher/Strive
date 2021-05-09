import { DomainError } from './communication-types';
import { EquipmentCommandAction } from './equipment-hub.types';
import { ProducerSource } from './store/webrtc/types';

export const equipmentCommandError: (
   message: string,
   action: EquipmentCommandAction,
   source: ProducerSource,
) => DomainError = (message, action, source) => ({
   code: 'Equipment/Command_Error',
   message: `An error occurred on executing "${action}" on ${source}: ${message}`,
   type: 'InternalServerError',
   fields: {
      message,
      action,
      source,
   },
});

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

export const mediaNotConnected: () => DomainError = () => ({
   code: 'UI/MediaNotConnected',
   message: 'Not connected',
   type: 'BadRequest',
});

export const equipmentKickedParticipantLeft: () => DomainError = () => ({
   code: 'UI/Equipment_ParticipantLeft',
   message: 'You left the conference using your main device.',
   type: 'Forbidden',
});
