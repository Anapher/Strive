import { ChangeStreamDto, SendChatMessageDto, CreateRoomDto, SwitchRoomDto } from './core-hub.types';
import { invoke, send } from './store/conference-signal/actions';

export const openConference = () => send('OpenConference');
export const closeConference = () => send('CloseConference');

export const createRooms = (rooms: CreateRoomDto[]) => send('CreateRooms', rooms);
export const removeRooms = (roomIds: string[]) => send('RemoveRooms', roomIds);
export const switchRoom = (dto: SwitchRoomDto) => send('SwitchRoom', dto);

export const changeSteam = (request: ChangeStreamDto) => send('ChangeStream', request);

export const loadFullChat = () => send('RequestChat');

export const sendChatMessage = (dto: SendChatMessageDto) => send('SendChatMessage', dto);

export const _getEquipmentToken = 'GetEquipmentToken';
export const getEquipmentToken = () => invoke(_getEquipmentToken);
