import { ChatMessageDto, SendChatMessageDto } from 'MyModels';
import { onEventOccurred, send, subscribeEvent } from 'src/store/conference-signal/actions';

export const loadFullChat = () => send('RequestChat');
export const sendChatMessage = (dto: SendChatMessageDto) => send('SendChatMessage', dto);

export const subscribeFullChat = () => subscribeEvent('Chat');
export const subscribeChatMessages = () => subscribeEvent('ChatMessage');

export const onFullChat = onEventOccurred<ChatMessageDto[]>('Chat');
export const onChatMessage = onEventOccurred<ChatMessageDto>('ChatMessage');
