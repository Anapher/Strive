import i18next, { FormatFunction } from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';
import de from 'src/assets/locales/de';
import en from 'src/assets/locales/en';
import { formatErrorMessage } from 'src/utils/error-utils';

const resources = {
   en,
   de,
};

// plural:
// {
//     // ...
//     "basket_delivered": "{{count}} basket delivered",
//     "basket_delivered_plural": "{{count}} baskets delivered"
//   }

// <p>{t("basket_delivered", { count: 2342 })}</p>
// ✋🏽Heads up » The counter variable must be called count, otherwise plural selection won’t work.

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
      supportedLngs: ['en', 'de'],
      ns: ['common', 'glossary', 'main'],
      defaultNS: 'main',
      nonExplicitSupportedLngs: true,
      interpolation: {
         escapeValue: false,
         format: formatInterpolation,
      },
   });

export default i18next;
