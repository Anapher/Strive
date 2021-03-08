import { Operation } from 'fast-json-patch';
import { DomainError } from './communication-types';
import {
   RoomCreationInfo,
   EquipmentCommand,
   EquipmentStatus,
   KickParticipantRequestDto,
   OpenBreakoutRoomsDto,
   SendChatMessageDto,
   SetTemporaryPermissionDto,
   SwitchRoomDto,
} from './core-hub.types';
import { RegisterEquipmentRequestDto } from './features/equipment/types';
import { connectSignal, invoke, onInvokeReturn } from './store/signal/actions';
import { ChangeProducerSourceDto, ChangeStreamDto } from './store/webrtc/types';

export const joinConference = (conferenceId: string, defaultEvents: string[], accessToken: string) =>
   connectSignal({ conferenceId, access_token: accessToken }, defaultEvents, { conferenceId });

export const joinConferenceAsEquipment = (conferenceId: string, defaultEvents: string[], token: string) =>
   connectSignal({ conferenceId, token }, defaultEvents, { conferenceId });

export const openConference = createHubFn('OpenConference');
export const closeConference = createHubFn('CloseConference');
export const kickParticipant = createHubFn<KickParticipantRequestDto>('KickParticipant');

export const createRooms = createHubFn<RoomCreationInfo[]>('CreateRooms');
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
export const equipmentErrorOccurred = createHubFn<DomainError>('EquipmentErrorOccurred');
export const equipmentUpdateStatus = createHubFn<EquipmentStatus>('EquipmentUpdateStatus');

export const changeStream = createHubFn<ChangeStreamDto>('ChangeStream');
export const changeProducerSource = createHubFn<ChangeProducerSourceDto>('ChangeProducerSource');

export const fetchPermissions = createHubFn<string | null>('FetchPermissions');
export const setTemporaryPermission = createHubFn<SetTemporaryPermissionDto>('SetTemporaryPermission');

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

   onEquipmentUpdated: 'OnEquipmentUpdated',
   onEquipmentCommand: 'OnEquipmentCommand',
   onRequestDisconnect: 'OnRequestDisconnect',
};
