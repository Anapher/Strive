import { useReactOidc } from '@axa-fr/react-oidc-context';
import { Dialog, DialogContent, Menu, MenuItem, MenuProps } from '@material-ui/core';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { useParams } from 'react-router-dom';
import DialogTitleWithClose from 'src/components/DialogTitleWithClose';
import * as coreHub from 'src/core-hub';
import { openDialogToPatchAsync } from 'src/features/create-conference/reducer';
import { openSettings } from 'src/features/settings/reducer';
import usePermission from 'src/hooks/usePermission';
import { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/permissions';
import { ConferenceRouteParams } from 'src/routes/types';
import Troubleshooting from '../troubleshoot/Troubleshooting';

export default function MobileLayoutActionsMenu(props: MenuProps & { onClose: () => void }) {
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const { logout } = useReactOidc();

   const { id: conferenceId } = useParams<ConferenceRouteParams>();

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => {
      dispatch(openSettings());
      props.onClose?.();
   };

   const handlePatchConference = () => {
      if (!conferenceId) {
         console.error('Conference id must not be null');
         return;
      }
      dispatch(openDialogToPatchAsync(conferenceId));
      props.onClose?.();
   };

   const [troubleshootOpen, setTroubleshootOpen] = useState(false);
   const handleCloseTroubleshoot = () => setTroubleshootOpen(false);
   const handleOpenTroubleshoot = () => {
      setTroubleshootOpen(true);
      props.onClose?.();
   };

   return (
      <>
         <Menu {...props}>
            <MenuItem onClick={handleOpenTroubleshoot}>{t('conference.troubleshooting.title')}</MenuItem>
            <MenuItem onClick={handleOpenSettings}>{t('common:settings')}</MenuItem>
            <MenuItem onClick={handlePatchConference}>{t('conference.appbar.change_conference_settings')}</MenuItem>

            {canCloseConference && (
               <MenuItem onClick={handleCloseConference}>{t('conference.appbar.close_conference')}</MenuItem>
            )}
            <MenuItem onClick={logout as any}>{t('common:sign_out')}</MenuItem>
         </Menu>

         <Dialog id="troubleshooting-dialog" open={troubleshootOpen} onClose={handleCloseTroubleshoot} fullScreen>
            <DialogTitleWithClose onClose={handleCloseTroubleshoot}>
               {t('conference.troubleshooting.title')}
            </DialogTitleWithClose>
            <DialogContent>
               <Troubleshooting />
            </DialogContent>
         </Dialog>
      </>
   );
}
