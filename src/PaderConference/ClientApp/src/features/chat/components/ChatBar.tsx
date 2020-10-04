import { Box, Divider, IconButton, makeStyles, Paper, Typography, TextField } from '@material-ui/core';
import React, { useEffect } from 'react';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import SendIcon from '@material-ui/icons/Send';
import * as actions from '../actions';
import { RootState } from 'src/store';
import { useDispatch, useSelector } from 'react-redux';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
   },
   chat: {
      backgroundColor: theme.palette.type === 'dark' ? '#303030' : 'red',
      flex: 1,
   },
}));

export default function ChatBar() {
   const classes = useStyles();
   const chat = useSelector<RootState>((state) => state.chat);
   const connected = useSelector<RootState>((state) => state.signalr.isConnected) as boolean;
   const dispatch = useDispatch();

   useEffect(() => {
      if (connected) {
         dispatch(actions.subscribeFullChat());
         dispatch(actions.subscribeChatMessages());
         dispatch(actions.loadFullChat());
      }
   }, [connected]);

   return (
      <Paper className={classes.root} elevation={4}>
         <Box display="flex" flexDirection="row" m={1} ml={2} justifyContent="space-between" alignItems="center">
            <Typography variant="h6">Chat</Typography>
            <IconButton aria-label="options">
               <MoreVertIcon fontSize="small" />
            </IconButton>
         </Box>
         <Divider orientation="horizontal" />
         <div className={classes.chat}></div>
         <Box m={1}>
            <TextField id="standard-basic" placeholder="Type your message..." fullWidth />
            <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
               <Typography>Vincent</Typography>
               <IconButton aria-label="options">
                  <SendIcon fontSize="small" />
               </IconButton>
            </Box>
         </Box>
      </Paper>
   );
}
