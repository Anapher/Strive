import { Operation } from 'fast-json-patch';
import appSettings from './config';
import {
   CreatePollDto,
   DeletePollDto,
   FetchChatMessagesDto,
   KickParticipantRequestDto,
   OpenBreakoutRoomsDto,
   RoomCreationInfo,
   SendChatMessageDto,
   SendEquipmentCommandDto,
   SetTemporaryPermissionDto,
   SetUserIsTypingDto,
   SubmitPollAnswerDto,
   SwitchRoomDto,
   UpdatePollStateDto,
   DeletePollAnswerDto,
} from './core-hub.types';
import { Scene } from './features/scenes/types';
import { WhiteboardPushActionDto } from './features/whiteboard/types';
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

export const changeProducerSource = createHubFn<ChangeProducerSourceRequest>('ChangeParticipantProducer');

export const fetchPermissions = createHubFn<string | null>('FetchPermissions');
export const setTemporaryPermission = createHubFn<SetTemporaryPermissionDto>('SetTemporaryPermission');

export const setScene = createHubFn<Scene>('SetScene');
export const setOverwrittenScene = createHubFn<Scene | null>('SetOverwrittenScene');
export const talkingStickEnqueue = createHubFn('TalkingStickEnqueue');
export const talkingStickDequeue = createHubFn('TalkingStickDequeue');
export const talkingStickTake = createHubFn('TalkingStickTake');
export const talkingStickReturn = createHubFn('TalkingStickReturn');
export const talkingStickPass = createHubFn<string>('TalkingStickPass');

export const createPoll = createHubFn<CreatePollDto>('CreatePoll');
export const submitPollAnswer = createHubFn<SubmitPollAnswerDto>('SubmitPollAnswer');
export const updatePollState = createHubFn<UpdatePollStateDto>('UpdatePollState');
export const deletePoll = createHubFn<DeletePollDto>('DeletePoll');
export const deletePollAnswer = createHubFn<DeletePollAnswerDto>('DeletePollAnswer');

export const createWhiteboard = createHubFn('CreateWhiteboard');
export const whiteboardPushAction = createHubFn<WhiteboardPushActionDto>('WhiteboardPushAction');
export const whiteboardUndo = createHubFn<string>('WhiteboardUndo');
export const whiteboardRedo = createHubFn<string>('WhiteboardRedo');

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
