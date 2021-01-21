import { RootState } from 'src/store';

export const selectParticipantsTyping = (state: RootState) => state.chat.chatInfo?.participantsTyping;

export const selectMessages = (state: RootState) => state.chat.messages;

export const selectFetchChatError = (state: RootState) => state.chat.fetchChatError;
