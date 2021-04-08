import { createSelector } from 'reselect';
import { RootState } from 'src/store';
import * as channelSerializer from './channel-serializer';

export const selectParticipantsTyping = (state: RootState, channelId: string) => {
   const channel = state.chat.channels?.[channelId];
   if (!channel) return [];
   return Object.keys(channel.participantsTyping) ?? [];
};

export const selectMessages = (state: RootState, channel: string) =>
   state.chat.channels?.[channel]?.viewModel?.messages;

export const selectAnnouncements = (state: RootState) => state.chat.announcements;

const selectChannelIds = (state: RootState) => (state.chat.channels ? Object.keys(state.chat.channels) : []);

export const selectChannels = createSelector(selectChannelIds, (channelIds) =>
   channelIds.map((x) => channelSerializer.decode(x)),
);

export const selectSelectedChannel = (state: RootState) => state.chat.selectedChannel;

export const selectShowChat = (state: RootState) => selectChannelIds(state).length > 0;
