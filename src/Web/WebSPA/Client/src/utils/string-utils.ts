import stopWordsDe from 'src/assets/stop_words/stop_words_de.json';
import stopWordsEn from 'src/assets/stop_words/stop_words_en.json';

export function formatBytes(bytes: number, decimals = 2) {
   if (bytes === 0) return '0 Bytes';

   const k = 1024;
   const dm = decimals < 0 ? 0 : decimals;
   const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

   const i = Math.floor(Math.log(bytes) / Math.log(k));

   return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
}

export function deleteStopwords(s: string, locale: string) {
   let words: string[];

   switch (locale.split('-')[0]) {
      case 'de':
         words = stopWordsDe;
         break;
      default:
         words = stopWordsEn;
         break;
   }

   const stringParts = s.replace('?', '').split(' ');
   const importantParts = stringParts.filter((x) => !words.includes(x.toLowerCase()));
   return importantParts.join(' ');
}

export function hashCode(str: string): number {
   // java String#hashCode
   let hash = 0;
   for (let i = 0; i < str.length; i++) {
      hash = str.charCodeAt(i) + ((hash << 5) - hash);
   }

   return hash;
}
