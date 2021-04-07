import { Dialog, DialogTitle, Typography } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ConferenceDataForm, mapDataToForm, mapFormToData } from '../form';
import { closeDialog, createConferenceAsync } from '../reducer';
import ConferenceCreatedView from './ConferenceCreatedView';
import CreateConferenceForm from './CreateConferenceForm';

function CreateConferenceDialog() {
   const dispatch = useDispatch();

   const { dialogOpen, createdConferenceId, isCreating, conferenceData, mode } = useSelector(
      (state: RootState) => state.createConference,
   );

   const handleSubmit = (data: ConferenceDataForm) => {
      const dto = mapFormToData(data);
      if (mode === 'create') {
         dispatch(createConferenceAsync(dto));
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
         <DialogTitle id="create-conference-dialog-title">Create a new conference</DialogTitle>
         {createdConferenceId ? (
            <ConferenceCreatedView conferenceId={createdConferenceId} />
         ) : (
            <>
               {conferenceData ? (
                  <CreateConferenceForm
                     onClose={handleClose}
                     isSubmitting={isCreating}
                     onSubmit={handleSubmit}
                     defaultValues={mapDataToForm(conferenceData)}
                  />
               ) : (
                  <div>
                     <Typography>Loading...</Typography>
                  </div>
               )}
            </>
         )}
      </Dialog>
   );
}

export default CreateConferenceDialog;
