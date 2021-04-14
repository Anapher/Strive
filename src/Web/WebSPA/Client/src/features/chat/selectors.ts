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

export const selectMessagesError = (state: RootState, channel: string) =>
   state.chat.channels?.[channel]?.viewModel?.messagesError;

export const selectAnnouncements = (state: RootState) => state.chat.announcements;

const selectChannelIds = (state: RootState) => {
   const synchronizedChannels = state.chat.channels ? Object.keys(state.chat.channels) : [];
   const privateChannels = state.chat.openedPrivateChats;

   return [...synchronizedChannels.filter((x) => !privateChannels.includes(x)), ...privateChannels];
};

const selectOpenedPrivateChats = (state: RootState) => state.chat.openedPrivateChats;

export const selectChannels = createSelector(
   selectChannelIds,
   selectOpenedPrivateChats,
   (channelIds, openedPrivateChats) => {
      const channels = channelIds.map<channelSerializer.ChatChannelWithId>((x) => channelSerializer.decode(x));

      const openedChannels = channels.filter((x) => x.type !== 'private' || openedPrivateChats.includes(x.id));
      const newPrivateChats = openedPrivateChats
         .filter((channelId) => !openedChannels.find((channel) => channel.id === channelId))
         .map((x) => channelSerializer.decode(x));

      return [...openedChannels, ...newPrivateChats];
   },
);

export const selectIsNewChannel = (state: RootState, channel: string) => !state.chat.channels?.[channel];

export const selectSelectedChannel = (state: RootState) => state.chat.selectedChannel;

export const selectShowChat = (state: RootState) => selectChannelIds(state).length > 0;

export const selectPrivateMessageEnabled = (state: RootState) => state.conference.conferenceState?.isPrivateChatEnabled;
