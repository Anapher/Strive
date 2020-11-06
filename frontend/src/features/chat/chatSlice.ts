import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { ChatMessageDto } from 'MyModels';
import { onInvokeReturn } from 'src/store/signal/actions';
import * as actions from './actions';
import * as coreHub from 'src/core-hub';

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
      [onInvokeReturn(coreHub._requestChat).type]: (state, action: PayloadAction<ChatMessageDto[]>) => {
         state.chat = action.payload;
      },
      [actions.onChatMessage.type]: (state, action: PayloadAction<ChatMessageDto>) => {
         state.chat?.push(action.payload);
      },
   },
});

export default chatSlice.reducer;
