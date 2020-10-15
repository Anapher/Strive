import {
   Box,
   Button,
   Dialog,
   DialogActions,
   DialogContent,
   DialogTitle,
   FormControlLabel,
   LinearProgress,
   makeStyles,
   Switch,
   TextField,
} from '@material-ui/core';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import to from 'src/utils/to';
import { closeCreateDialog, createConferenceAsync } from '../createConferenceSlice';
import CreateConferenceForm, { CreateConferenceFormState } from './CreateConferenceForm';

const useStyles = makeStyles({
   conferenceUrlField: {
      flex: 1,
      marginRight: 16,
   },
});

function CreateConferenceDialog() {
   const [allowUsersUnmute, setAllowUsersUnmute] = useState(true);
   const classes = useStyles();
   const dispatch = useDispatch();

   const { dialogOpen, createdConferenceId, isCreating } = useSelector((state: RootState) => state.createConference);

   const handleCreate = () => dispatch(createConferenceAsync({ settings: { allowUsersToUnmute: allowUsersUnmute } }));
   const handleClose = () => dispatch(closeCreateDialog());

   const form = useForm<CreateConferenceFormState>({
      mode: 'onChange',
      defaultValues: { conferenceType: 'class', repeat: false, schedule: false, repeatCron: '0 0 15 ? * MON *' },
   });

   return (
      <Dialog
         open={dialogOpen}
         onClose={handleClose}
         aria-labelledby="start-conference-dialog-title"
         fullWidth
         maxWidth="sm"
      >
         <DialogTitle id="start-conference-dialog-title">Start a new conference</DialogTitle>
         <DialogContent>
            {createdConferenceId ? (
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
            ) : (
               <CreateConferenceForm form={form} />
            )}
         </DialogContent>
         <DialogActions>
            <Button autoFocus onClick={handleClose} color="primary" disabled={isCreating}>
               {createdConferenceId ? 'Close' : 'Cancel'}
            </Button>
            {!createdConferenceId && (
               <Button onClick={handleCreate} color="primary" autoFocus disabled={isCreating}>
                  Start
               </Button>
            )}
         </DialogActions>
         {isCreating && <LinearProgress />}
      </Dialog>
   );
}

export default CreateConferenceDialog;
