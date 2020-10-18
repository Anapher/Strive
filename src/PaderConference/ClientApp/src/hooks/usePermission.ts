import { useSelector } from 'react-redux';
import { PermissionValue } from 'src/features/create-conference/types';
import { RootState } from 'src/store';

export const CONFERENCE_CAN_OPEN_AND_CLOSE = newPerm<boolean>('conference.canOpenAndClose');
export const ROOMS_CAN_CREATE_REMOVE = newPerm<boolean>('rooms.canCreateAndRemove');

export const MEDIA_CAN_SHARE_AUDIO = newPerm<boolean>('media.canShareAudio');
export const MEDIA_CAN_SHARE_SCREEN = newPerm<boolean>('media.canShareScreen');
export const MEDIA_CAN_SHARE_WEBCAM = newPerm<boolean>('media.canShareWebcam');

export default function usePermission<T>(perm: Permission<T>): T | undefined {
   const permissions = useSelector((state: RootState) => state.conference.myPermissions);
   if (!permissions) return undefined;

   return permissions[perm.key] as any;
}

type Permission<T> = {
   key: string;
};

function newPerm<T extends PermissionValue>(key: string): Permission<T> {
   return { key };
}
