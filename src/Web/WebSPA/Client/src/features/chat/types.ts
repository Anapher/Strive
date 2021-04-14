import { DomainError } from 'src/communication-types';
import { ChatMessageDto } from 'src/core-hub.types';

export type ChatSynchronizedObjectViewModel = {
   viewModel?: ChannelViewModel;
};

export type ChannelViewModel = {
   messages: ChatMessageDto[];
   newMessages: boolean;
   messagesError?: DomainError;
};
