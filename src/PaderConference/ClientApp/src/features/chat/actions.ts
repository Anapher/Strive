import { ChatMessageDto } from 'MyModels';
import { onEventOccurred, subscribeEvent } from 'src/store/conference-signal/actions';

export const subscribeFullChat = () => subscribeEvent('Chat');
export const subscribeChatMessages = () => subscribeEvent('ChatMessage');

export const onFullChat = onEventOccurred<ChatMessageDto[]>('Chat');
export const onChatMessage = onEventOccurred<ChatMessageDto>('ChatMessage');
