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
import { RootState } from 'pader-conference';
import React, { useState } from 'react';
import { connect } from 'react-redux';
import * as actions from '../actions';

const useStyles = makeStyles({
   conferenceUrlField: {
      flex: 1,
      marginRight: 16,
   },
});

const mapStateToProps = (state: RootState) => ({
   open: state.conference.createDialogOpen,
   loading: state.conference.isCreatingConference,
   createdConference: state.conference.createdConferenceId,
});

const dispatchProps = {
   createConference: actions.createConferenceAsync.request,
   onClose: actions.closeCreateDialog,
};

type Props = ReturnType<typeof mapStateToProps> & typeof dispatchProps;

function CreateConferenceDialog({ open, loading, createdConference, createConference, onClose }: Props) {
   const [allowUsersUnmute, setAllowUsersUnmute] = useState(true);
   const handleCreate = () => createConference({ settings: { allowUsersToUnmute: allowUsersUnmute } });

   const classes = useStyles();

   return (
      <Dialog open={open} onClose={onClose} aria-labelledby="start-conference-dialog-title" fullWidth maxWidth="sm">
         <DialogTitle id="start-conference-dialog-title">Start a new conference</DialogTitle>
         <DialogContent>
            {!createdConference ? (
               <Box display="flex" flexDirection="row" alignItems="center">
                  <TextField
                     variant="outlined"
                     label="Conference Url"
                     InputProps={{ readOnly: true }}
                     value={createdConference}
                     className={classes.conferenceUrlField}
                  />
                  <Button variant="contained">Join</Button>
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
            <Button autoFocus onClick={onClose} color="primary" disabled={loading}>
               {createdConference ? 'Close' : 'Cancel'}
            </Button>
            {!createdConference && (
               <Button onClick={handleCreate} color="primary" autoFocus disabled={loading}>
                  Start
               </Button>
            )}
         </DialogActions>
         {loading && <LinearProgress />}
      </Dialog>
   );
}

export default connect(mapStateToProps, dispatchProps)(CreateConferenceDialog);
