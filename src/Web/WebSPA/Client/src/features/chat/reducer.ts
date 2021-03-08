import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { SendingMode } from 'src/core-hub.types';
import { CHAT } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import * as actions from './actions';
import { ChatMessageDto, ChatSynchronizedObject } from './types';

export type ChatState = Readonly<{
   messages: ChatMessageDto[] | null;
   chatInfo: ChatSynchronizedObject | null;
   fetchChatError: DomainError | null;
   sendingMode: SendingMode | null;
}>;

const initialState: ChatState = {
   messages: null,
   chatInfo: null,
   fetchChatError: null,
   sendingMode: null,
};

const chatSlice = createSlice({
   name: 'chat',
   initialState,
   reducers: {
      setSendingMode(state, { payload }: PayloadAction<SendingMode | null>) {
         state.sendingMode = payload;
      },
   },
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
      ...synchronizeObjectState({ type: 'multiple', baseId: CHAT, propertyName: 'chatInfo' }),
   },
});

export const { setSendingMode } = chatSlice.actions;

export default chatSlice.reducer;
