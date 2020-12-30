import { DomainError } from './communication-types';
import { ProducerSource } from './participant';

export const internalError: (info: string) => DomainError = (info) => ({
   code: 'SFU/Internal_Error',
   message: `An internal error occurred in SFU: ${info}`,
   type: 'InternalServerError',
});

export const connectionNotFound: (connectionId: string) => DomainError = (id) => ({
   code: 'SFU/Connection_Not_Found',
   message: `The connection ${id} was not found.`,
   type: 'BadRequest',
});

export const transportNotFound: (transportId: string) => DomainError = (id) => ({
   code: 'SFU/Transport_Not_Found',
   message: `The transport ${id} was not found.`,
   type: 'BadRequest',
});

export const invalidProducerKind: (source: string, kind: string) => DomainError = (source, kind) => ({
   code: 'SFU/Invalid_Producer_Kind',
   message: `Cannot create producer for source ${source} with kind ${kind}.`,
   type: 'BadRequest',
});

export const conferenceNotFound: (conferenceId: string) => DomainError = (id) => ({
   code: 'SFU/Conference_Not_Found',
   message: `The conference with id ${id} was not found.`,
   type: 'BadRequest',
});

export const streamNotFound: (type: string, streamId: string) => DomainError = (type, id) => ({
   code: 'SFU/Stream_Not_Found',
   message: `The ${type} with id ${id} was not found.`,
   type: 'BadRequest',
});

export const producerSourceNotFound: (source: ProducerSource) => DomainError = (source) => ({
   code: 'SFU/Producer_Source_Not_Found',
   message: `The producer source ${source} was not found.`,
   type: 'BadRequest',
});

export const participantNotFound: (participantId: string) => DomainError = (id) => ({
   code: 'SFU/Participant_Not_Found',
   message: `The participant ${id} was not found.`,
   type: 'BadRequest',
});
