import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { ChatMessageDto, FetchChatMessagesDto } from 'src/core-hub.types';
import { CHAT, ChatSynchronizedObject } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { InvokeMethodMeta } from 'src/store/signal/types';
import * as actions from './actions';
import { ChatSynchronizedObjectViewModel } from './types';
import { mergeChatMessages } from './utils';

export type ChatState = Readonly<{
   channels: { [channel: string]: ChatSynchronizedObject & ChatSynchronizedObjectViewModel } | null;
   selectedChannel: string | null;
   announcements: ChatMessageDto[];

   openedPrivateChats: string[];
}>;

const initialState: ChatState = {
   channels: null,
   selectedChannel: null,
   announcements: [],

   openedPrivateChats: [],
};

const chatSlice = createSlice({
   name: 'chat',
   initialState,
   reducers: {
      setSelectedChannel(state, { payload }: PayloadAction<string | null>) {
         state.selectedChannel = payload;

         if (payload) {
            const channel = state.channels?.[payload];
            if (channel?.viewModel) channel.viewModel.newMessages = false;
         }
      },
      addAnnouncement(state, { payload }: PayloadAction<ChatMessageDto>) {
         state.announcements.push(payload);
      },
      removeAnnouncement(state, { payload }: PayloadAction<ChatMessageDto>) {
         state.announcements = state.announcements.filter((x) => x.id !== payload.id || x.channel !== payload.channel);
      },
      openPrivateChat(state, { payload }: PayloadAction<string>) {
         if (!state.openedPrivateChats.includes(payload)) state.openedPrivateChats.push(payload);
      },
      closePrivateChat(state, { payload }: PayloadAction<string>) {
         state.openedPrivateChats = state.openedPrivateChats.filter((x) => x !== payload);
         if (state.selectedChannel === payload) {
            state.selectedChannel = Object.keys(state.channels ?? {})[0];
         }
      },
      clearChat(state) {
         if (state.channels) {
            for (const channel of Object.values(state.channels)) {
               if (channel.viewModel) channel.viewModel.messages = [];
            }
         }
      },
   },
   extraReducers: {
      [coreHub.fetchChatMessages.returnAction]: (
         state,
         {
            payload,
            meta,
         }: PayloadAction<SuccessOrError<ChatMessageDto[]>, string, InvokeMethodMeta<FetchChatMessagesDto>>,
      ) => {
         if (!state.channels) return;

         const channelId = meta.request.channel;
         const channel = state.channels[channelId];

         if (!channel) return;

         if (payload.success) {
            if (!channel.viewModel) {
               channel.viewModel = { messages: payload.response, newMessages: false, messagesFetched: true };
            } else {
               channel.viewModel.messages = mergeChatMessages(channel.viewModel.messages, payload.response);
            }
            channel.viewModel.messagesError = undefined;
         } else {
            if (!channel.viewModel) {
               channel.viewModel = {
                  messages: [],
                  newMessages: false,
                  messagesError: payload.error,
                  messagesFetched: false,
               };
            } else {
               channel.viewModel.messagesError = payload.error;
            }
         }
      },
      [actions.onChatMessage.type]: (state, { payload }: PayloadAction<ChatMessageDto>) => {
         const channel = state.channels?.[payload.channel];
         if (!channel) return;

         if (!channel.viewModel) channel.viewModel = { messages: [], newMessages: false, messagesFetched: false };
         channel.viewModel.messages.push(payload);

         if (state.selectedChannel !== payload.channel) channel.viewModel.newMessages = true;
      },
      ...synchronizeObjectState({ type: 'multiple', baseId: CHAT, propertyName: 'channels' }),
   },
});

export const {
   setSelectedChannel,
   addAnnouncement,
   removeAnnouncement,
   openPrivateChat,
   closePrivateChat,
   clearChat,
} = chatSlice.actions;

export default chatSlice.reducer;
