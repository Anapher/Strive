import i18next from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import en from 'src/assets/locales/en';
import de from 'src/assets/locales/de';

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
      },
   });

export default i18next;
