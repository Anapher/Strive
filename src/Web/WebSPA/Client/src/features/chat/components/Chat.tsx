import { makeStyles, Paper } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { SendChatMessageDto } from 'src/core-hub.types';
import { RootState } from 'src/store';
import {
   selectIsNewChannel,
   selectMessages,
   selectMessagesError,
   selectMessagesFetched,
   selectParticipantsTyping,
} from '../selectors';
import ChatMessageList from './ChatMessageList';
import NewChat from './NewChat';
import ParticipantsTyping from './ParticipantsTyping';
import SendMessageForm from './SendMessageForm';

const useStyles = makeStyles((theme) => ({
   chat: {
      backgroundColor: theme.palette.type === 'dark' ? 'rgb(32, 32, 34)' : 'red',
      flex: 1,
      minHeight: 0,
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
}));

type Props = {
   channel: string;
   participantId?: string | null;
   participantColors: { [id: string]: string };
};

export default function Chat({ channel, participantId, participantColors }: Props) {
   const messages = useSelector((state: RootState) => selectMessages(state, channel));
   const messagesFetched = useSelector((state: RootState) => selectMessagesFetched(state, channel));
   const error = useSelector((state: RootState) => selectMessagesError(state, channel));
   const dispatch = useDispatch();
   const classes = useStyles();

   const isNewChannel = useSelector((state: RootState) => selectIsNewChannel(state, channel));

   const participantsTyping = useSelector((state: RootState) => selectParticipantsTyping(state, channel));
   const isMeTyping = !!participantId && participantsTyping.includes(participantId);

   const handleFetchChat = () => {
      dispatch(coreHub.fetchChatMessages({ channel, start: 0, end: -1 }));
   };

   useEffect(() => {
      if (!messagesFetched && !isNewChannel) {
         handleFetchChat();
      }
   }, [messages, channel, isNewChannel]);

   const handleSendMessage = (message: SendChatMessageDto) => {
      dispatch(coreHub.sendChatMessage(message));
   };

   return (
      <div className={classes.chat}>
         {isNewChannel ? (
            <NewChat />
         ) : (
            <ChatMessageList
               messages={messages}
               participantColors={participantColors}
               participantId={participantId}
               error={error}
               onRetry={handleFetchChat}
            />
         )}

         <Paper>
            <ParticipantsTyping
               participantsTyping={participantsTyping.filter((x) => x !== participantId)}
               participantColors={participantColors}
            />
            <SendMessageForm channel={channel} isTyping={isMeTyping} onSendMessage={handleSendMessage} />
         </Paper>
      </div>
   );
}
