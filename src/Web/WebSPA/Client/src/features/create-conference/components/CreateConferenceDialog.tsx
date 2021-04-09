import { Dialog, DialogTitle, makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ConferenceDataForm, mapDataToForm, mapFormToData } from '../form';
import { closeDialog, createConferenceAsync, patchConferenceAsync } from '../reducer';
import ConferenceCreatedView from './ConferenceCreatedView';
import CreateConferenceForm from './CreateConferenceForm';
import { compare } from 'fast-json-patch';
import CreateConferenceFormSkeleton from './CreateConferenceFormSkeleton';

const useStyles = makeStyles({
   dialogContent: {
      height: 496,
   },
});

function CreateConferenceDialog() {
   const dispatch = useDispatch();
   const classes = useStyles();

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
      >
         <DialogTitle id="create-conference-dialog-title">
            {mode === 'patch' ? 'Change conference settings' : 'Create a new conference'}
         </DialogTitle>
         {createdConferenceId && mode !== 'patch' ? (
            <ConferenceCreatedView conferenceId={createdConferenceId} />
         ) : (
            <div className={classes.dialogContent}>
               {conferenceData ? (
                  <CreateConferenceForm
                     onClose={handleClose}
                     isSubmitting={isCreating}
                     onSubmit={handleSubmit}
                     defaultValues={mapDataToForm(conferenceData)}
                     mode={mode}
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
