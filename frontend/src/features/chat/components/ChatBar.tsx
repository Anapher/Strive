import { Box, Divider, IconButton, makeStyles, Paper, Typography } from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { selectMyParticipantId } from 'src/features/auth/selectors';
import { RootState } from 'src/store';
import * as actions from '../actions';
import { selectParticipantsTyping } from '../selectors';
import ChatMessageList from './ChatMessageList';
import SendMessageForm from './SendMessageForm';
import UsersTyping from './UsersTyping';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
      overflowY: 'hidden',
   },
   chat: {
      backgroundColor: theme.palette.type === 'dark' ? '#303030' : 'red',
      flex: 1,
      minHeight: 0,
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
   const participantsTyping = useSelector(selectParticipantsTyping);
   const participantId = useSelector(selectMyParticipantId);

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
            <UsersTyping participantsTyping={participantsTyping?.filter((x) => x !== participantId)} />
            <SendMessageForm
               onSendMessage={handleSendMessage}
               isTyping={!!participantsTyping && !!participantId && participantsTyping.includes(participantId)}
            />
         </Box>
      </Paper>
   );
}
