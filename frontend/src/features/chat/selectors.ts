import { RootState } from 'src/store';

export const selectParticipantsTyping = (state: RootState) => state.chat.chatInfo?.participantsTyping;
