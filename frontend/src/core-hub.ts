import { Operation } from 'fast-json-patch';
import {
   SendChatMessageDto,
   CreateRoomDto,
   SwitchRoomDto,
   EquipmentCommand,
   EquipmentStatus,
   OpenBreakoutRoomsDto,
} from './core-hub.types';
import { RegisterEquipmentRequestDto } from './features/equipment/types';
import { connectSignal, invoke } from './store/signal/actions';
import { IRestError } from './utils/error-result';

export const joinConference = (conferenceId: string, defaultEvents: string[], accessToken: string) =>
   connectSignal({ conferenceId, access_token: accessToken }, defaultEvents, { conferenceId });

export const joinConferenceAsEquipment = (conferenceId: string, defaultEvents: string[], token: string) =>
   connectSignal({ conferenceId, token }, defaultEvents, { conferenceId });

export const openConference = () => invoke('OpenConference');
export const closeConference = () => invoke('CloseConference');

export const createRooms = (rooms: CreateRoomDto[]) => invoke('CreateRooms', rooms);
export const removeRooms = (roomIds: string[]) => invoke('RemoveRooms', roomIds);
export const switchRoom = (dto: SwitchRoomDto) => invoke('SwitchRoom', dto);

export const openBreakoutRooms = (dto: OpenBreakoutRoomsDto) => invoke('OpenBreakoutRooms', dto);
export const closeBreakoutRooms = () => invoke('CloseBreakoutRooms');
export const changeBreakoutRooms = (dto: Operation[] /** for BreakoutRoomsOptions */) =>
   invoke('ChangeBreakoutRooms', dto);

export const _requestChat = 'RequestChat';
export const requestChat = () => invoke(_requestChat);

export const sendChatMessage = (dto: SendChatMessageDto) => invoke('SendChatMessage', dto);
export const setUserTyping = (typing: boolean) => invoke('SetUserIsTyping', typing);

export const _getEquipmentToken = 'GetEquipmentToken';
export const getEquipmentToken = () => invoke(_getEquipmentToken);

export const registerEquipment = (dto: RegisterEquipmentRequestDto) => invoke('RegisterEquipment', dto);
export const sendEquipmentCommand = (dto: EquipmentCommand) => invoke('SendEquipmentCommand', dto);
export const equipmentErrorOccurred = (dto: IRestError) => invoke('EquipmentErrorOccurred', dto);
export const equipmentUpdateStatus = (dto: EquipmentStatus) => invoke('EquipmentUpdateStatus', dto);

export const changeStream = 'ChangeStream';

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
