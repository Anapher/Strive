import { DomainError } from './communication-types';

export const signalrError: (error: any) => DomainError = (error) => ({
   code: 'UI/Signal_Error',
   message: `An error occurred on invoking signalr method: ${error}.`,
   type: 'InternalServerError',
});
