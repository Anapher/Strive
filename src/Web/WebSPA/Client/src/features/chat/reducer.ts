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
}>;

const initialState: ChatState = {
   channels: null,
   selectedChannel: null,
   announcements: [],
};

const chatSlice = createSlice({
   name: 'chat',
   initialState,
   reducers: {
      setSelectedChannel(state, { payload }: PayloadAction<string | null>) {
         state.selectedChannel = payload;
      },
      addAnnouncement(state, { payload }: PayloadAction<ChatMessageDto>) {
         state.announcements.push(payload);
      },
      removeAnnouncement(state, { payload }: PayloadAction<ChatMessageDto>) {
         state.announcements = state.announcements.filter((x) => x.id !== payload.id || x.channel !== payload.channel);
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
         if (payload.success) {
            if (!state.channels) return;

            const channelId = meta.request.channel;
            const channel = state.channels[channelId];

            if (!channel) return;

            if (!channel.viewModel) {
               channel.viewModel = { messages: payload.response };
            } else {
               channel.viewModel.messages = mergeChatMessages(channel.viewModel.messages, payload.response);
            }
         }
      },
      [actions.onChatMessage.type]: (state, { payload }: PayloadAction<ChatMessageDto>) => {
         const channel = state.channels?.[payload.channel];
         if (!channel) return;

         if (!channel.viewModel) channel.viewModel = { messages: [] };
         channel.viewModel.messages.push(payload);
      },
      ...synchronizeObjectState({ type: 'multiple', baseId: CHAT, propertyName: 'channels' }),
   },
});

export const { setSelectedChannel, addAnnouncement, removeAnnouncement } = chatSlice.actions;

export default chatSlice.reducer;
