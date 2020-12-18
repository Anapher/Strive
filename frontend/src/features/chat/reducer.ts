import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import * as actions from './actions';
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
      [coreHub.requestChat.returnAction]: (state, { payload }: PayloadAction<SuccessOrError<ChatMessageDto[]>>) => {
         if (payload.success) state.chat = payload.response;
      },
      [actions.onChatMessage.type]: (state, action: PayloadAction<ChatMessageDto>) => {
         state.chat?.push(action.payload);
      },
      ...createSynchronizeObjectReducer('chatInfo'),
   },
});

export default chatSlice.reducer;
