import i18next, { FormatFunction } from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';
import de from 'src/assets/locales/de';
import en from 'src/assets/locales/en';
import { formatErrorMessage } from 'src/utils/error-utils';

type LanguageInfo = {
   id: string;
   name: string;
};

const resources = {
   en,
   de,
};

export const supportedLanguages: LanguageInfo[] = [
   { id: 'en', name: 'English' },
   { id: 'de', name: 'Deutsch' },
];

const formatInterpolation: FormatFunction = (value: any, format?: string) => {
   switch (format) {
      case 'error':
         return formatErrorMessage(value);
      default:
         return value;
   }
};

i18next
   .use(initReactI18next)
   .use(LanguageDetector)
   .init({
      resources,
      fallbackLng: 'en',
      supportedLngs: supportedLanguages.map((x) => x.id),
      ns: ['common', 'glossary', 'main'],
      defaultNS: 'main',
      nonExplicitSupportedLngs: true,
      interpolation: {
         escapeValue: false,
         format: formatInterpolation,
      },
   });

export default i18next;
