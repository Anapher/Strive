import { useEffect } from 'react';

const APP_NAME = 'Strive';

export default function usePageTitle(title?: string | null) {
   useEffect(() => {
      const previousTitle = document.title;

      return () => {
         document.title = previousTitle;
      };
   }, []);

   useEffect(() => {
      if (title) {
         document.title = `${APP_NAME} | ${title}`;
      } else {
         document.title = APP_NAME;
      }
   }, [title]);
}
