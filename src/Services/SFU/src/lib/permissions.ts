export const MEDIA_CAN_SHARE_AUDIO = newPerm<boolean>('audio');
export const MEDIA_CAN_SHARE_WEBCAM = newPerm<boolean>('webcam');
export const MEDIA_CAN_SHARE_SCREEN = newPerm<boolean>('screen');

export type Permission<T> = {
   key: string;
};

export type PermissionValue = number | string | boolean;

function newPerm<T extends PermissionValue>(key: string): Permission<T> {
   return { key };
}
