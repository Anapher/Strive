import { DomainError } from 'src/communication-types';
import i18next from 'i18next';

export function formatErrorMessage(error: DomainError): string {
   return i18next.t(`errors:${error.code}`, error.message, { fields: error.fields });
}
