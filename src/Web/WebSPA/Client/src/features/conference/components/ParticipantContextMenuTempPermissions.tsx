import { Typography } from '@material-ui/core';
import { ToggleButtonGroup, ToggleButton } from '@material-ui/lab';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { selectParticipantTempPermissions } from '../selectors';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import MicIcon from '@material-ui/icons/Mic';
import VideocamIcon from '@material-ui/icons/Videocam';
import { setTemporaryPermission } from 'src/core-hub';
import { MEDIA_CAN_SHARE_AUDIO, MEDIA_CAN_SHARE_SCREEN, MEDIA_CAN_SHARE_WEBCAM } from 'src/permissions';
import { showMessage } from 'src/store/notifier/actions';

type Props = {
   participantId: string;
};

const observedPermissions = [MEDIA_CAN_SHARE_AUDIO.key, MEDIA_CAN_SHARE_WEBCAM.key, MEDIA_CAN_SHARE_SCREEN.key];

export default function ParticipantContextMenuTempPermissions({ participantId }: Props) {
   const tempPermissions = useSelector((state: RootState) => selectParticipantTempPermissions(state, participantId));
   const dispatch = useDispatch();

   const currentTruthyPermissions = tempPermissions
      ? Object.entries(tempPermissions)
           .filter(([key, value]) => observedPermissions.includes(key) && value)
           .map(([key]) => key)
      : [];

   const handleSetTempPermission = (key: string, value?: boolean) => {
      dispatch(
         showMessage({
            type: 'loading',
            message: 'Update permissions...',
            dismissOn: {
               type: setTemporaryPermission.returnAction,
               successMessage: value ? 'Permission granted' : 'Permission revoked',
            },
         }),
      );
      dispatch(setTemporaryPermission({ participantId, permissionKey: key, value }));
   };

   const handleChangePermissions = (_: React.MouseEvent<HTMLElement>, newPermissions: string[]) => {
      for (const permission of observedPermissions) {
         if (newPermissions.includes(permission) && !currentTruthyPermissions.includes(permission)) {
            handleSetTempPermission(permission, true);
         } else if (!newPermissions.includes(permission) && currentTruthyPermissions.includes(permission)) {
            handleSetTempPermission(permission);
         }
      }
   };

   return (
      <div
         style={{
            display: 'flex',
            justifyContent: 'space-between',
            paddingLeft: 16,
            paddingRight: 16,
            paddingTop: 8,
            paddingBottom: 8,
            alignItems: 'center',
         }}
      >
         <Typography variant="subtitle1">Permissions:</Typography>
         <ToggleButtonGroup
            aria-label="temporary permissions"
            size="small"
            value={currentTruthyPermissions}
            onChange={handleChangePermissions}
         >
            <ToggleButton value={MEDIA_CAN_SHARE_SCREEN.key} aria-label="allow screen sharing">
               <DesktopWindowsIcon fontSize="small" />
            </ToggleButton>
            <ToggleButton value={MEDIA_CAN_SHARE_WEBCAM.key} aria-label="allow webcam">
               <VideocamIcon fontSize="small" />
            </ToggleButton>
            <ToggleButton value={MEDIA_CAN_SHARE_AUDIO.key} aria-label="allow microphone">
               <MicIcon fontSize="small" />
            </ToggleButton>
         </ToggleButtonGroup>
      </div>
   );
}
