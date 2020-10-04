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
import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import to from 'src/utils/to';
import { closeCreateDialog, createConferenceAsync } from '../createConferenceSlice';

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
                  <Button variant="contained" {...to('/c/' + createdConferenceId)} color="primary">
                     Join
                  </Button>
               </Box>
            ) : (
               <FormControlLabel
                  control={
                     <Switch
                        checked={allowUsersUnmute}
                        onChange={() => setAllowUsersUnmute(!allowUsersUnmute)}
                        name="allowUsersUnmute"
                        color="primary"
                     />
                  }
                  label="Allow users to unmute themselves"
               />
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
