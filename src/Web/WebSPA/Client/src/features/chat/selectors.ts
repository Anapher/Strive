import { createSelector } from 'reselect';
import { RootState } from 'src/store';
import * as channelSerializer from './channel-serializer';

export const selectParticipantsTyping = (state: RootState) => state.chat.channels?.participantsTyping;

export const selectMessages = (state: RootState, channel: string) => state.chat.channels?.[channel].viewModel?.messages;

// export const selectFetchChatError = (state: RootState) => state.chat.fetchChatError;

export const selectChannelIds = (state: RootState) => (state.chat.channels ? Object.keys(state.chat.channels) : []);

export const selectChannels = createSelector(selectChannelIds, (channelIds) => {
   return channelIds.map((x) => channelSerializer.decode(x));
});

export const selectSelectedChannel = (state: RootState) => state.chat.selectedChannel;
