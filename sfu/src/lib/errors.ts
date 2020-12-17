import { DomainError } from './communication-types';

export const internalError: (info: string) => DomainError = (info) => ({
   code: 1001,
   message: `An internal error occurred in SFU: ${info}`,
   type: 'internalError',
});

export const connectionNotFound: (connectionId: string) => DomainError = (id) => ({
   code: 1002,
   message: `The connection ${id} was not found.`,
   type: 'requestError',
});

export const transportNotFound: (transportId: string) => DomainError = (id) => ({
   code: 1003,
   message: `The transport ${id} was not found.`,
   type: 'requestError',
});

export const invalidProducerKind: (source: string, kind: string) => DomainError = (source, kind) => ({
   code: 1004,
   message: `Cannot create producer for source ${source} with kind ${kind}.`,
   type: 'requestError',
});

export const conferenceNotFound: (conferenceId: string) => DomainError = (id) => ({
   code: 1005,
   message: `The conference with id ${id} was not found.`,
   type: 'requestError',
});

export const streamNotFound: (type: string, streamId: string) => DomainError = (type, id) => ({
   code: 1006,
   message: `The ${type} with id ${id} was not found.`,
   type: 'requestError',
});

export const participantNotFound: (participantId: string) => DomainError = (id) => ({
   code: 10000,
   message: `The participant ${id} was not found.`,
   type: 'requestError',
});
