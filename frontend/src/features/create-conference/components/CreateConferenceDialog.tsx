import {
   Box,
   Button,
   Dialog,
   DialogActions,
   DialogContent,
   DialogTitle,
   LinearProgress,
   makeStyles,
   TextField,
} from '@material-ui/core';
import { DateTime } from 'luxon';
import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
import { RootState } from 'src/store';
import to from 'src/utils/to';
import { closeCreateDialog, createConferenceAsync, loadDefaultPermissionsAsync } from '../reducer';
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

   const { dialogOpen, createdConferenceId, isCreating, defaultPermissions } = useSelector(
      (state: RootState) => state.createConference,
   );

   useEffect(() => {
      if (!defaultPermissions) dispatch(loadDefaultPermissionsAsync());
   }, []);

   const handleCreate = (data: ConferenceData) => {
      // const dto = mapFormToDto(data);
      // dispatch(dispatch(createConferenceAsync(dto)));
   };

   const handleClose = () => dispatch(closeCreateDialog());

   const user = useSelector(selectAccessToken);

   const form = useForm<ConferenceData>({
      mode: 'onChange',
      shouldUnregister: false,
   });

   const { handleSubmit, formState } = form;

   return (
      <Dialog
         open={dialogOpen}
         onClose={handleClose}
         aria-labelledby="create-conference-dialog-title"
         fullWidth
         maxWidth="sm"
      >
         <DialogTitle id="create-conference-dialog-title">Create a new conference</DialogTitle>
         <form onSubmit={handleSubmit(handleCreate)}>
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
               </DialogContent>
            ) : (
               <CreateConferenceForm form={form} />
            )}

            <DialogActions>
               <Button autoFocus onClick={handleClose} color="primary" disabled={isCreating}>
                  {createdConferenceId ? 'Close' : 'Cancel'}
               </Button>
               {!createdConferenceId && (
                  <Button type="submit" color="primary" autoFocus disabled={isCreating || !formState.isValid}>
                     Create
                  </Button>
               )}
            </DialogActions>
            {isCreating && <LinearProgress />}
         </form>
      </Dialog>
   );
}

export default CreateConferenceDialog;
