declare module 'MyModels' {
   export type SendChatMessageDto = {
      message: string;
      filter?: ChatMessageFilter;
   };

   export type ChatMessageFilter = {
      include?: string[];
      exclude?: string[];
   };

   export type ChatMessageDto = {
      messageId: number;
      participantId: string;
      message: string;
      timestamp: string;
   };
}
