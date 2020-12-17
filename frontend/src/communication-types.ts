export type ErrorType = 'serviceError' | 'ui';

export type DomainError = {
   type: ErrorType;
   message: string;
   code: number;
   fields?: { [key: string]: string };
};

export type SuccessOrErrorSucceeded<T> = {
   response: T;
   success: true;
};

export type SuccessOrErrorFailed = {
   error: DomainError;
   success: false;
};

export type SuccessOrError<T> = SuccessOrErrorSucceeded<T> | SuccessOrErrorFailed;
