import { Operation } from 'fast-json-patch';
import {
   CreateRoomDto,
   EquipmentCommand,
   EquipmentStatus,
   OpenBreakoutRoomsDto,
   SendChatMessageDto,
   SwitchRoomDto,
} from './core-hub.types';
import { RegisterEquipmentRequestDto } from './features/equipment/types';
import { connectSignal, invoke, onInvokeReturn } from './store/signal/actions';
import { ChangeStreamDto } from './store/webrtc/types';
import { IRestError } from './utils/error-result';

export const joinConference = (conferenceId: string, defaultEvents: string[], accessToken: string) =>
   connectSignal({ conferenceId, access_token: accessToken }, defaultEvents, { conferenceId });

export const joinConferenceAsEquipment = (conferenceId: string, defaultEvents: string[], token: string) =>
   connectSignal({ conferenceId, token }, defaultEvents, { conferenceId });

export const openConference = createHubFn('OpenConference');
export const closeConference = createHubFn('CloseConference');

export const createRooms = createHubFn<CreateRoomDto[]>('CreateRooms');
export const removeRooms = createHubFn<string[]>('RemoveRooms');
export const switchRoom = createHubFn<SwitchRoomDto>('SwitchRoom');

export const openBreakoutRooms = createHubFn<OpenBreakoutRoomsDto>('OpenBreakoutRooms');
export const closeBreakoutRooms = createHubFn('CloseBreakoutRooms');
export const changeBreakoutRooms = createHubFn<Operation[] /** for BreakoutRoomsOptions */>('ChangeBreakoutRooms');

export const requestChat = createHubFn('RequestChat');
export const sendChatMessage = createHubFn<SendChatMessageDto>('SendChatMessage');
export const setUserTyping = createHubFn<boolean>('SetUserIsTyping');

export const getEquipmentToken = createHubFn('GetEquipmentToken');

export const registerEquipment = createHubFn<RegisterEquipmentRequestDto>('RegisterEquipment');
export const sendEquipmentCommand = createHubFn<EquipmentCommand>('SendEquipmentCommand');
export const equipmentErrorOccurred = createHubFn<IRestError>('EquipmentErrorOccurred');
export const equipmentUpdateStatus = createHubFn<EquipmentStatus>('EquipmentUpdateStatus');

export const changeStream = createHubFn<ChangeStreamDto>('ChangeStream');

export function createHubFn<TArg = void>(name: string) {
   const actionCreator = function (arg: TArg) {
      return invoke(name)(arg);
   };

   actionCreator.hubName = name;
   actionCreator.action = invoke(name).type;
   actionCreator.returnAction = onInvokeReturn(name).type;
   return actionCreator;
}

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
