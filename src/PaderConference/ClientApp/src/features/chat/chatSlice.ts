import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { ChatMessageDto } from 'MyModels';
import * as actions from './actions';

export type ChatState = Readonly<{
   chat: ChatMessageDto[] | null;
}>;

const initialState: ChatState = {
   chat: null,
};

const chatSlice = createSlice({
   name: 'chat',
   initialState,
   reducers: {
      clear(state) {
         state.chat = null;
      },
   },
   extraReducers: {
      [actions.onFullChat.type]: (state, action: PayloadAction<ChatMessageDto[]>) => {
         state.chat = action.payload;
      },
      [actions.onChatMessage.type]: (state, action: PayloadAction<ChatMessageDto>) => {
         state.chat?.push(action.payload);
      },
   },
});

export default chatSlice.reducer;
