// Conference
export const CONFERENCE_CAN_OPEN_AND_CLOSE: Permission = { key: 'conference.canOpenAndClose', type: 'bool' };
export const CONFERENCE_CAN_RAISE_HAND: BoolPermission = { key: 'conference.canRaiseHand', type: 'bool' };
export const CONFERENCE_CAN_KICK_PARTICIPANT: BoolPermission = { key: 'conference.canKickParticipant', type: 'bool' };

// Permissions
export const PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION: BoolPermission = {
   key: 'permissions.canGiveTemporaryPermission',
   type: 'bool',
};
export const PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS: BoolPermission = {
   key: 'permissions.canSeeAnyParticipantsPermissions',
   type: 'bool',
};

// Chat
export const CHAT_CAN_SEND_CHAT_MESSAGE: BoolPermission = { key: 'chat.canSendMessage', type: 'bool' };
export const CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE: BoolPermission = { key: 'chat.canSendPrivateMessage', type: 'bool' };
export const CHAT_CAN_SEND_ANONYMOUS_MESSAGE: BoolPermission = { key: 'chat.canSendAnonymousMessage', type: 'bool' };

// Media
export const MEDIA_CAN_SHARE_AUDIO: BoolPermission = { key: 'media.canShareAudio', type: 'bool' };
export const MEDIA_CAN_SHARE_SCREEN: BoolPermission = { key: 'media.canShareScreen', type: 'bool' };
export const MEDIA_CAN_SHARE_WEBCAM: BoolPermission = { key: 'media.canShareWebcam', type: 'bool' };
export const MEDIA_CAN_CHANGE_PARTICIPANTS_PRODUCER: BoolPermission = {
   key: 'media.canChangeOtherParticipantsProducers',
   type: 'bool',
};

// Rooms
export const ROOMS_CAN_CREATE_REMOVE: BoolPermission = { key: 'rooms.canCreateAndRemove', type: 'bool' };
export const ROOMS_CAN_SWITCH_ROOM: BoolPermission = { key: 'rooms.canSwitchRoom', type: 'bool' };

// Scenes
export const SCENES_CAN_SET_SCENE: BoolPermission = { key: 'scenes.canSetScene', type: 'bool' };

export type BoolPermission = {
   key: string;
   type: 'bool';
};

export type IntPermission = {
   key: string;
   type: 'int';
};

export type Permission = BoolPermission | IntPermission;
