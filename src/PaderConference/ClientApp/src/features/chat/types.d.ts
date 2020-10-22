declare module 'MyModels' {
   export type ChatMessageDto = {
      messageId: number;
      participantId: string;
      message: string;
      timestamp: string;
   };
}
