import { Box, Button, DialogActions, DialogContent, makeStyles, TextField } from '@material-ui/core';
import React from 'react';
import { useDispatch } from 'react-redux';
import { closeDialog } from '../reducer';
import to from 'src/utils/to';

const useStyles = makeStyles((theme) => ({
   conferenceUrlField: {
      flex: 1,
      marginRight: theme.spacing(2),
   },
}));

type Props = {
   conferenceId: string;
};

export default function ConferenceCreatedView({ conferenceId }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();

   const handleClose = () => dispatch(closeDialog());

   return (
      <DialogContent>
         <Box display="flex" flexDirection="row" alignItems="center">
            <TextField
               variant="outlined"
               label="Conference Url"
               InputProps={{ readOnly: true }}
               value={new URL('/c/' + conferenceId, document.baseURI).href}
               className={classes.conferenceUrlField}
            />
            <Button variant="contained" {...to('/c/' + conferenceId)} onClick={handleClose} color="primary">
               Join
            </Button>
         </Box>
         <DialogActions>
            <Button onClick={handleClose} color="primary">
               Close
            </Button>
         </DialogActions>
      </DialogContent>
   );
}
