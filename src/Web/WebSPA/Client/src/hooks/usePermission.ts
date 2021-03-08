import { useSelector } from 'react-redux';
import { Permission, BoolPermission } from 'src/permissions';
import { RootState } from 'src/store';

export default function usePermission<T extends Permission>(
   perm: T,
): (T extends BoolPermission ? boolean : number) | undefined {
   const permissions = useSelector((state: RootState) => state.conference.myPermissions);
   if (!permissions) return undefined;

   return permissions.permissions[perm.key] as any;
}
