import { Dialog, DialogTitle } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { closeSettings } from '../reducer';
import Settings from './Settings';

export default function SettingsDialog() {
   const open = useSelector((state: RootState) => state.settings.open);
   const dispatch = useDispatch();

   const handleClose = () => dispatch(closeSettings());

   return (
      <Dialog open={open} onClose={handleClose} aria-labelledby="settings-dialog-title" fullWidth maxWidth="md">
         <DialogTitle id="settings-dialog-title">Settings</DialogTitle>
         <Settings />
      </Dialog>
   );
}
