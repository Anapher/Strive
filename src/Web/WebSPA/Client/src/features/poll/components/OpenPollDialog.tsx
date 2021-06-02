import { Dialog, DialogTitle } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { CreatePollDto } from 'src/core-hub.types';
import { RootState } from 'src/store';
import { setCreationDialogOpen } from '../reducer';
import OpenPollDialogForm from './OpenPollDialogForm';
import * as coreHub from 'src/core-hub';

export default function OpenPollDialog() {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const open = useSelector((state: RootState) => state.poll.creationDialogOpen);
   const handleClose = () => dispatch(setCreationDialogOpen(false));

   const handleSubmit = (dto: CreatePollDto) => {
      dispatch(coreHub.createPoll(dto));
   };

   return (
      <Dialog
         open={open}
         onClose={handleClose}
         aria-labelledby="poll-dialog-title"
         fullWidth
         maxWidth="sm"
         scroll="paper"
      >
         <DialogTitle id="poll-dialog-title">{t('conference.poll.create_dialog.title')}</DialogTitle>
         <OpenPollDialogForm open={open} onSubmit={handleSubmit} />
      </Dialog>
   );
}
