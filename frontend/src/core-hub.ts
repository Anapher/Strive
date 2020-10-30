import { SendChatMessageDto, CreateRoomDto, SwitchRoomDto, EquipmentCommand, EquipmentStatus } from './core-hub.types';
import { RegisterEquipmentRequestDto } from './features/equipment/types';
import { connectSignal, invoke, send } from './store/signal/actions';
import { IRestError } from './utils/error-result';

export const joinConference = (conferenceId: string, defaultEvents: string[], accessToken: string) =>
   connectSignal({ conferenceId, access_token: accessToken }, defaultEvents, { conferenceId });

export const joinConferenceAsEquipment = (conferenceId: string, defaultEvents: string[], token: string) =>
   connectSignal({ conferenceId, token }, defaultEvents, { conferenceId });

export const openConference = () => send('OpenConference');
export const closeConference = () => send('CloseConference');

export const createRooms = (rooms: CreateRoomDto[]) => send('CreateRooms', rooms);
export const removeRooms = (roomIds: string[]) => send('RemoveRooms', roomIds);
export const switchRoom = (dto: SwitchRoomDto) => send('SwitchRoom', dto);

export const loadFullChat = () => send('RequestChat');

export const sendChatMessage = (dto: SendChatMessageDto) => send('SendChatMessage', dto);

export const _getEquipmentToken = 'GetEquipmentToken';
export const getEquipmentToken = () => invoke(_getEquipmentToken);

export const registerEquipment = (dto: RegisterEquipmentRequestDto) => send('RegisterEquipment', dto);
export const sendEquipmentCommand = (dto: EquipmentCommand) => send('SendEquipmentCommand', dto);
export const equipmentErrorOccurred = (dto: IRestError) => send('EquipmentErrorOccurred', dto);
export const equipmentUpdateStatus = (dto: EquipmentStatus) => send('EquipmentUpdateStatus', dto);

export const events = {
   onConnectionError: 'OnConnectionError',

   onSynchronizeObjectState: 'OnSynchronizeObjectState',
   onSynchronizedObjectUpdated: 'OnSynchronizedObjectUpdated',

   chatMessage: 'ChatMessage',
   chat: 'Chat',

   onPermissionsUpdated: 'OnPermissionsUpdated',
   onEquipmentUpdated: 'OnEquipmentUpdated',
   onEquipmentCommand: 'OnEquipmentCommand',
};
