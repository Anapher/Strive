import { DomainError } from 'src/communication-types';
import i18next from 'i18next';

export function formatErrorMessage(error: DomainError): string {
   if (!error.code) {
      if (error.message) return error.message;
      return error.toString();
   }

   return i18next.t(`errors:${error.code}`, error.message, { fields: error.fields });
}
