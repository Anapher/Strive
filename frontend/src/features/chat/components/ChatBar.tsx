import { Box, Divider, IconButton, makeStyles, Paper, Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import * as actions from '../actions';
import { RootState } from 'src/store';
import { useDispatch, useSelector } from 'react-redux';
import ChatMessageList from './ChatMessageList';
import SendMessageForm from './SendMessageForm';
import * as coreHub from 'src/core-hub';

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
   const { chat } = useSelector((state: RootState) => state.chat);
   const participants = useSelector((state: RootState) => state.conference.participants);
   const connected = useSelector((state: RootState) => state.signalr.isConnected);
   const dispatch = useDispatch();

   useEffect(() => {
      if (connected) {
         dispatch(actions.subscribeChatMessages());
         dispatch(coreHub.requestChat());
      }
   }, [connected]);

   const handleSendMessage = (message: string) => dispatch(coreHub.sendChatMessage({ message }));

   return (
      <Paper className={classes.root} elevation={4}>
         <Box display="flex" flexDirection="row" m={1} ml={2} justifyContent="space-between" alignItems="center">
            <Typography variant="h6">Chat</Typography>
            <IconButton aria-label="options">
               <MoreVertIcon fontSize="small" />
            </IconButton>
         </Box>
         <Divider orientation="horizontal" />
         <div className={classes.chat}>
            <ChatMessageList chat={chat} participants={participants} />
         </div>
         <Box m={1}>
            <SendMessageForm onSendMessage={handleSendMessage} />
         </Box>
      </Paper>
   );
}
