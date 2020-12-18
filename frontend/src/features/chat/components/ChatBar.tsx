import { Box, Divider, makeStyles, Paper, Typography } from '@material-ui/core';
import React, { useEffect, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { SendChatMessageDto } from 'src/core-hub.types';
import { selectMyParticipantId } from 'src/features/auth/selectors';
import { selectParticipants } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import * as actions from '../actions';
import { hashCode, numberToColor } from '../color-utils';
import { selectFetchChatError, selectMessages, selectParticipantsTyping } from '../selectors';
import ChatMessageList from './ChatMessageList';
import SendMessageForm from './SendMessageForm';
import UsersTyping from './UsersTyping';

const useStyles = makeStyles((theme) => ({
   root: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      overflowY: 'hidden',
      borderTopRightRadius: 0,
      borderBottomRightRadius: 0,
      borderColor: theme.palette.divider,
      borderWidth: '0px 0px 0px 1px',
      borderStyle: 'solid',
   },
   chat: {
      backgroundColor: theme.palette.type === 'dark' ? 'rgb(32, 32, 34)' : 'red',
      flex: 1,
      minHeight: 0,
   },
}));

export default function ChatBar() {
   const classes = useStyles();
   const messages = useSelector(selectMessages);
   const participants = useSelector(selectParticipants);
   const connected = useSelector((state: RootState) => state.signalr.isConnected);
   const fetchChatError = useSelector(selectFetchChatError);

   const dispatch = useDispatch();

   const handleFetchChat = () => dispatch(coreHub.requestChat());

   useEffect(() => {
      if (connected) {
         dispatch(actions.subscribeChatMessages());
         handleFetchChat();
      }
   }, [connected]);

   const handleSendMessage = (dto: SendChatMessageDto) => dispatch(coreHub.sendChatMessage(dto));
   const participantsTyping = useSelector(selectParticipantsTyping);
   const participantId = useSelector(selectMyParticipantId);

   const participantColors = useMemo(
      () =>
         Object.fromEntries(
            participants?.map((x) => [x.participantId, numberToColor(hashCode(x.participantId))]) ?? [],
         ),
      [participants],
   );

   return (
      <Paper className={classes.root} elevation={4}>
         <Box display="flex" flexDirection="row" m={1} ml={2} justifyContent="space-between" alignItems="center">
            <Typography variant="h6">Chat</Typography>
         </Box>
         <Divider orientation="horizontal" />
         <div className={classes.chat}>
            <ChatMessageList
               chat={messages}
               participants={participants}
               participantId={participantId}
               participantColors={participantColors}
               error={fetchChatError}
               onRetry={handleFetchChat}
            />
         </div>
         <Box m={1}>
            <UsersTyping
               participantsTyping={participantsTyping?.filter((x) => x !== participantId)}
               participantColors={participantColors}
            />
            <SendMessageForm
               onSendMessage={handleSendMessage}
               isTyping={!!participantsTyping && !!participantId && participantsTyping.includes(participantId)}
            />
         </Box>
      </Paper>
   );
}
