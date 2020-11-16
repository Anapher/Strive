declare module 'MyModels' {
   export type ChatMessageDto = {
      messageId: number;
      participantId: string;
      message: string;
      timestamp: string;
   };
}

declare module 'emoji-regex/RGI_Emoji' {
   function emojiRegex(): RegExp;

   export = emojiRegex;
}
