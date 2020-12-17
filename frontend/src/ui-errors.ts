import { DomainError } from './communication-types';

export const SIGNALR_ERROR = 0;

export const signalrError: (error: any) => DomainError = (error) => ({
   code: SIGNALR_ERROR,
   message: `An error occurred on invoking signalr method: ${error}.`,
   type: 'ui',
});
