import { makeStyles, Typography } from '@material-ui/core';
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
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles((theme) => ({
   menuEntry: {
      display: 'flex',
      justifyContent: 'space-between',
      padding: theme.spacing(1, 2),
      alignItems: 'center',
   },
}));

type Props = {
   participantId: string;
};

const observedPermissions = [MEDIA_CAN_SHARE_AUDIO.key, MEDIA_CAN_SHARE_WEBCAM.key, MEDIA_CAN_SHARE_SCREEN.key];

export default function ParticipantContextMenuTempPermissions({ participantId }: Props) {
   const { t } = useTranslation();
   const tempPermissions = useSelector((state: RootState) => selectParticipantTempPermissions(state, participantId));
   const dispatch = useDispatch();
   const classes = useStyles();

   const currentTruthyPermissions = tempPermissions
      ? Object.entries(tempPermissions)
           .filter(([key, value]) => observedPermissions.includes(key) && value)
           .map(([key]) => key)
      : [];

   const handleSetTempPermission = (key: string, value?: boolean) => {
      dispatch(
         showMessage({
            type: 'loading',
            message: t('conference.notifications.update_permissions.loading'),
            dismissOn: {
               type: setTemporaryPermission.returnAction,
               successMessage: value
                  ? t('conference.notifications.update_permissions.success_granted')
                  : t('conference.notifications.update_permissions.success_revoked'),
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
      <div className={classes.menuEntry}>
         <Typography variant="subtitle1">{t('common:permissions')}:</Typography>
         <ToggleButtonGroup
            aria-label={t('glossary:temporary_permissions')}
            size="small"
            value={currentTruthyPermissions}
            onChange={handleChangePermissions}
         >
            <ToggleButton
               value={MEDIA_CAN_SHARE_SCREEN.key}
               aria-label={t('conference.participant_context_menu.allow_screen')}
            >
               <DesktopWindowsIcon fontSize="small" />
            </ToggleButton>
            <ToggleButton
               value={MEDIA_CAN_SHARE_WEBCAM.key}
               aria-label={t('conference.participant_context_menu.allow_webcam')}
            >
               <VideocamIcon fontSize="small" />
            </ToggleButton>
            <ToggleButton
               value={MEDIA_CAN_SHARE_AUDIO.key}
               aria-label={t('conference.participant_context_menu.allow_mic')}
            >
               <MicIcon fontSize="small" />
            </ToggleButton>
         </ToggleButtonGroup>
      </div>
   );
}
