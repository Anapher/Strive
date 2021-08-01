import { Dialog, makeStyles, useMediaQuery, useTheme } from '@material-ui/core';
import { compare } from 'fast-json-patch';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import DialogTitleWithClose from 'src/components/DialogTitleWithClose';
import { RootState } from 'src/store';
import { ConferenceDataForm, mapDataToForm, mapFormToData } from '../form';
import { closeDialog, createConferenceAsync, patchConferenceAsync } from '../reducer';
import ConferenceCreatedView from './ConferenceCreatedView';
import CreateConferenceForm from './CreateConferenceForm';
import CreateConferenceFormSkeleton from './CreateConferenceFormSkeleton';

const useStyles = makeStyles((theme) => ({
   dialog: {
      display: 'flex',
      flexDirection: 'column',
   },
   dialogContent: {
      minHeight: 0,

      [theme.breakpoints.down('sm')]: {
         flex: 1,
      },
      [theme.breakpoints.up('md')]: {
         height: 496,
      },
   },
}));

function CreateConferenceDialog() {
   const dispatch = useDispatch();
   const theme = useTheme();
   const classes = useStyles();
   const { t } = useTranslation();

   const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
   const showCloseInTitle = fullScreen;

   const { dialogOpen, createdConferenceId, isCreating, conferenceData, mode } = useSelector(
      (state: RootState) => state.createConference,
   );

   const handleSubmit = (data: ConferenceDataForm) => {
      const dto = mapFormToData(data);
      if (mode === 'create') {
         dispatch(createConferenceAsync(dto));
      } else if (mode === 'patch') {
         if (!conferenceData || !createdConferenceId) {
            console.error('When patching, conferenceData and createdConferenceId must not be null');
            return;
         }

         const patch = compare(conferenceData, dto);
         dispatch(patchConferenceAsync({ conferenceId: createdConferenceId, patch }));
      }
   };
   const handleClose = () => dispatch(closeDialog());

   return (
      <Dialog
         open={dialogOpen}
         onClose={handleClose}
         aria-labelledby="create-conference-dialog-title"
         fullWidth
         maxWidth="sm"
         scroll="paper"
         fullScreen={fullScreen}
         className={classes.dialog}
      >
         <DialogTitleWithClose id="create-conference-dialog-title" onClose={handleClose} showClose={showCloseInTitle}>
            {t(
               mode === 'patch'
                  ? 'dialog_create_conference.title_mode_patch'
                  : 'dialog_create_conference.title_mode_create',
            )}
         </DialogTitleWithClose>
         {createdConferenceId && mode !== 'patch' ? (
            <ConferenceCreatedView conferenceId={createdConferenceId} showClose={!showCloseInTitle} />
         ) : (
            <div className={classes.dialogContent}>
               {conferenceData ? (
                  <CreateConferenceForm
                     onClose={handleClose}
                     isSubmitting={isCreating}
                     onSubmit={handleSubmit}
                     defaultValues={mapDataToForm(conferenceData)}
                     mode={mode}
                     conferenceId={createdConferenceId}
                     showClose={!showCloseInTitle}
                  />
               ) : (
                  <CreateConferenceFormSkeleton />
               )}
            </div>
         )}
      </Dialog>
   );
}

export default CreateConferenceDialog;
