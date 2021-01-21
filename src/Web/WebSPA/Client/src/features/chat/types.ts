import { ParticipantRef, SendingMode } from 'src/core-hub.types';

export type ChatSynchronizedObject = {
   participantsTyping: string[];
};

export type ChatMessageDto = {
   messageId: number;
   from?: ParticipantRef;
   message: string;
   mode?: SendingMode;
   timestamp: string;
};
