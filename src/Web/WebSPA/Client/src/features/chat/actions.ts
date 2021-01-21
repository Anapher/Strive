import { events } from 'src/core-hub';
import { onEventOccurred, subscribeEvent } from 'src/store/signal/actions';
import { ChatMessageDto } from './types';

export const subscribeChatMessages = () => subscribeEvent(events.chatMessage);

export const onChatMessage = onEventOccurred<ChatMessageDto>(events.chatMessage);
