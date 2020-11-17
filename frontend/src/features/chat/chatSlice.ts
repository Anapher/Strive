import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { onInvokeReturn } from 'src/store/signal/actions';
import * as actions from './actions';
import * as coreHub from 'src/core-hub';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { ChatMessageDto, ChatSynchronizedObject } from './types';

export type ChatState = Readonly<{
   chat: ChatMessageDto[] | null;
   chatInfo: ChatSynchronizedObject | null;
}>;

const initialState: ChatState = {
   chat: null,
   chatInfo: null,
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
      ...createSynchronizeObjectReducer('chatInfo'),
   },
});

export default chatSlice.reducer;
