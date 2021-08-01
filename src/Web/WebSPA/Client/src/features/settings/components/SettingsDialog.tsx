import { Dialog, useMediaQuery, useTheme } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import DialogTitleWithClose from 'src/components/DialogTitleWithClose';
import { RootState } from 'src/store';
import { closeSettings } from '../reducer';
import Settings from './Settings';

export default function SettingsDialog() {
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const open = useSelector((state: RootState) => state.settings.open);

   const handleClose = () => dispatch(closeSettings());

   const theme = useTheme();
   const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));

   return (
      <Dialog
         open={open}
         onClose={handleClose}
         aria-labelledby="settings-dialog-title"
         fullWidth
         maxWidth="md"
         fullScreen={fullScreen}
      >
         <DialogTitleWithClose onClose={handleClose} id="settings-dialog-title">
            {t('common:settings')}
         </DialogTitleWithClose>
         <Settings />
      </Dialog>
   );
}
