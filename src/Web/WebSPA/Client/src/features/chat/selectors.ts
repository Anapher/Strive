import { createSelector } from 'reselect';
import { RootState } from 'src/store';
import * as channelSerializer from './channel-serializer';

const getChannelParticipantsTyping = (state: RootState, channelId: string) =>
   state.chat.channels?.[channelId]?.participantsTyping;

export const selectParticipantsTyping = createSelector(getChannelParticipantsTyping, (channel) => {
   if (!channel) return [];
   return Object.keys(channel) ?? [];
});

export const selectMessages = (state: RootState, channel: string) =>
   state.chat.channels?.[channel]?.viewModel?.messages;

export const selectMessagesFetched = (state: RootState, channel: string) =>
   state.chat.channels?.[channel]?.viewModel?.messagesFetched;

export const selectMessagesError = (state: RootState, channel: string) =>
   state.chat.channels?.[channel]?.viewModel?.messagesError;

export const selectAnnouncements = (state: RootState) => state.chat.announcements;

export const selectOpenedPrivateChats = (state: RootState) => state.chat.openedPrivateChats;
const selectSynchronizedChannels = (state: RootState) => state.chat.channels;

const selectChannelIds = createSelector(
   selectSynchronizedChannels,
   selectOpenedPrivateChats,
   (channels, privateChannels) => {
      const synchronizedChannels = channels ? Object.keys(channels) : [];

      return [...synchronizedChannels.filter((x) => !privateChannels.includes(x)), ...privateChannels];
   },
);

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
