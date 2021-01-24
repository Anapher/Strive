export type ErrorType = 'BadRequest' | 'Conflict' | 'InternalServerError' | 'Forbidden' | 'NotFound';

export type DomainError = {
   type: ErrorType;
   message: string;
   code: string;
   fields?: { [key: string]: string };
};

export type SuccessOrErrorSucceeded<T> = {
   response: T;
   success: true;
};

export type SuccessOrErrorSucceededWithoutResult = {
   success: true;
};

export type SuccessOrErrorFailed = {
   error: DomainError;
   success: false;
};

export type SuccessOrError<T = never> =
   | ([T] extends [never] ? SuccessOrErrorSucceededWithoutResult : SuccessOrErrorSucceeded<T>)
   | SuccessOrErrorFailed;
