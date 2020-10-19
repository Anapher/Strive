export const CONFERENCE_CAN_OPEN_AND_CLOSE = newPerm<boolean>('conference.canOpenAndClose');
export const ROOMS_CAN_CREATE_REMOVE = newPerm<boolean>('rooms.canCreateAndRemove');

export const MEDIA_CAN_SHARE_AUDIO = newPerm<boolean>('media.canShareAudio');
export const MEDIA_CAN_SHARE_SCREEN = newPerm<boolean>('media.canShareScreen');
export const MEDIA_CAN_SHARE_WEBCAM = newPerm<boolean>('media.canShareWebcam');

export type Permission<T> = {
   key: string;
};

export type PermissionValue = number | string | boolean;

function newPerm<T extends PermissionValue>(key: string): Permission<T> {
   return { key };
}
