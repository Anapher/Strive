import { events } from 'src/core-hub';
import { ChatMessageDto } from 'src/core-hub.types';
import { onEventOccurred, subscribeEvent } from 'src/store/signal/actions';

export const subscribeChatMessages = () => subscribeEvent(events.chatMessage);

export const onChatMessage = onEventOccurred<ChatMessageDto>(events.chatMessage);
