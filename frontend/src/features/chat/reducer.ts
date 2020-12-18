import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import * as actions from './actions';
import { ChatMessageDto, ChatSynchronizedObject } from './types';

export type ChatState = Readonly<{
   messages: ChatMessageDto[] | null;
   chatInfo: ChatSynchronizedObject | null;
   fetchChatError: DomainError | null;
}>;

const initialState: ChatState = {
   messages: null,
   chatInfo: null,
   fetchChatError: null,
};

const chatSlice = createSlice({
   name: 'chat',
   initialState,
   reducers: {},
   extraReducers: {
      [coreHub.requestChat.returnAction]: (state, { payload }: PayloadAction<SuccessOrError<ChatMessageDto[]>>) => {
         if (payload.success) {
            state.messages = payload.response;
         } else {
            state.fetchChatError = payload.error;
         }
      },
      [coreHub.requestChat.action]: (state) => {
         state.fetchChatError = null;
      },
      [actions.onChatMessage.type]: (state, action: PayloadAction<ChatMessageDto>) => {
         state.messages?.push(action.payload);
      },
      ...createSynchronizeObjectReducer('chatInfo'),
   },
});

export default chatSlice.reducer;
