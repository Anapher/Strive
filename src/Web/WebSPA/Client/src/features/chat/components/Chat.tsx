import { Box, makeStyles } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { SendChatMessageDto } from 'src/core-hub.types';
import { Participant } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { selectMessages, selectParticipantsTyping } from '../selectors';
import ChatMessageList from './ChatMessageList';
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
   participants: Participant[] | null;
   participantId?: string | null;
   participantColors: { [id: string]: string };
};

export default function Chat({ channel, participants, participantId, participantColors }: Props) {
   const messages = useSelector((state: RootState) => selectMessages(state, channel));
   const dispatch = useDispatch();
   const classes = useStyles();

   const participantsTyping = useSelector((state: RootState) => selectParticipantsTyping(state, channel));
   const isMeTyping = !!participantId && participantsTyping.includes(participantId);

   const handleFetchChat = () => {
      dispatch(coreHub.fetchChatMessages({ channel, start: 0, end: -1 }));
   };

   useEffect(() => {
      if (!messages) {
         handleFetchChat();
      }
   }, [messages]);

   const handleSendMessage = (message: SendChatMessageDto) => {
      dispatch(coreHub.sendChatMessage(message));
   };

   return (
      <div className={classes.chat}>
         <ChatMessageList
            messages={messages}
            participantColors={participantColors}
            participants={participants}
            participantId={participantId}
            error={null}
            onRetry={() => {
               console.log('test');
            }}
         />
         <Box>
            <SendMessageForm channel={channel} isTyping={isMeTyping} onSendMessage={handleSendMessage} />
         </Box>
      </div>
   );
}