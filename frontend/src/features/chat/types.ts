export type ChatSynchronizedObject = {
   participantsTyping: string[];
};

export type ChatMessageDto = {
   messageId: number;
   participantId: string;
   message: string;
   timestamp: string;
};
