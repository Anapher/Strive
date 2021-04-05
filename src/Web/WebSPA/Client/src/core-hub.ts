import { Operation } from 'fast-json-patch';
import appSettings from './config';
import {
   FetchChatMessagesDto,
   KickParticipantRequestDto,
   OpenBreakoutRoomsDto,
   RoomCreationInfo,
   SendChatMessageDto,
   SendEquipmentCommandDto,
   SetSceneDto,
   SetTemporaryPermissionDto,
   SetUserIsTypingDto,
   SwitchRoomDto,
} from './core-hub.types';
import { connectSignal, invoke, onInvokeReturn } from './store/signal/actions';
import { ChangeProducerSourceRequest } from './store/webrtc/types';

export const joinConference = (conferenceId: string, defaultEvents: string[], accessToken: string) =>
   connectSignal(appSettings.signalrHubUrl, { conferenceId, access_token: accessToken }, defaultEvents, {
      conferenceId,
   });

export const openConference = createHubFn('OpenConference');
export const closeConference = createHubFn('CloseConference');
export const kickParticipant = createHubFn<KickParticipantRequestDto>('KickParticipant');

export const createRooms = createHubFn<RoomCreationInfo[]>('CreateRooms');
export const removeRooms = createHubFn<string[]>('RemoveRooms');
export const switchRoom = createHubFn<SwitchRoomDto>('SwitchRoom');

export const openBreakoutRooms = createHubFn<OpenBreakoutRoomsDto>('OpenBreakoutRooms');
export const closeBreakoutRooms = createHubFn('CloseBreakoutRooms');
export const changeBreakoutRooms = createHubFn<Operation[] /** for BreakoutRoomsOptions */>('ChangeBreakoutRooms');

export const fetchChatMessages = createHubFn<FetchChatMessagesDto>('FetchChatMessages');
export const sendChatMessage = createHubFn<SendChatMessageDto>('SendChatMessage');
export const setUserTyping = createHubFn<SetUserIsTypingDto>('SetUserIsTyping');

export const getEquipmentToken = createHubFn('GetEquipmentToken');

export const sendEquipmentCommand = createHubFn<SendEquipmentCommandDto>('SendEquipmentCommand');

export const changeProducerSource = createHubFn<ChangeProducerSourceRequest>('ChangeProducerSource');

export const fetchPermissions = createHubFn<string | null>('FetchPermissions');
export const setTemporaryPermission = createHubFn<SetTemporaryPermissionDto>('SetTemporaryPermission');

export const setScene = createHubFn<SetSceneDto>('SetScene');

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
   onRequestDisconnect: 'OnRequestDisconnect',
   onEquipmentError: 'OnEquipmentError',
};
