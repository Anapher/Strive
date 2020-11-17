/// <reference types="react-scripts" />

declare module '*.mp3' {
   const src: string;
   export default src;
}

declare module 'emoji-regex/RGI_Emoji' {
   function emojiRegex(): RegExp;

   export = emojiRegex;
}
