export type ErrorType = 'serviceError';

export type DomainError = {
   type: ErrorType;
   message: string;
   code: number;
   fields?: { [key: string]: string };
};

export type Size = {
   width: number;
   height: number;
};
