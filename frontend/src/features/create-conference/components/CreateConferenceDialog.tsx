import {
   Box,
   Button,
   Dialog,
   DialogActions,
   DialogContent,
   DialogTitle,
   makeStyles,
   TextField,
   Typography,
} from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
import { RootState } from 'src/store';
import to from 'src/utils/to';
import { mapDataToForm } from '../form';
import { closeDialog } from '../reducer';
import { ConferenceData } from '../types';
import CreateConferenceForm from './CreateConferenceForm';

const useStyles = makeStyles((theme) => ({
   conferenceUrlField: {
      flex: 1,
      marginRight: theme.spacing(2),
   },
}));

function CreateConferenceDialog() {
   const classes = useStyles();
   const dispatch = useDispatch();

   const { dialogOpen, createdConferenceId, isCreating, conferenceData, mode } = useSelector(
      (state: RootState) => state.createConference,
   );

   const handleCreate = (data: ConferenceData) => {
      // const dto = mapFormToDto(data);
      // dispatch(dispatch(createConferenceAsync(dto)));
   };

   const handleClose = () => dispatch(closeDialog());

   const user = useSelector(selectAccessToken);

   return (
      <Dialog
         open={dialogOpen}
         onClose={handleClose}
         aria-labelledby="create-conference-dialog-title"
         fullWidth
         maxWidth="sm"
      >
         <DialogTitle id="create-conference-dialog-title">Create a new conference</DialogTitle>
         {createdConferenceId ? (
            <DialogContent>
               <Box display="flex" flexDirection="row" alignItems="center">
                  <TextField
                     variant="outlined"
                     label="Conference Url"
                     InputProps={{ readOnly: true }}
                     value={new URL('/c/' + createdConferenceId, document.baseURI).href}
                     className={classes.conferenceUrlField}
                  />
                  <Button
                     variant="contained"
                     {...to('/c/' + createdConferenceId)}
                     onClick={() => console.log('on join')}
                     color="primary"
                  >
                     Join
                  </Button>
               </Box>
               <DialogActions>
                  <Button onClick={handleClose} color="primary">
                     Close
                  </Button>
               </DialogActions>
            </DialogContent>
         ) : (
            <>
               {conferenceData ? (
                  <CreateConferenceForm
                     onClose={handleClose}
                     isSubmitting={isCreating}
                     onSubmit={handleCreate}
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
