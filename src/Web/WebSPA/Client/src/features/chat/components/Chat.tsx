import { Box } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { SendChatMessageDto } from 'src/core-hub.types';
import { Participant } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { selectMessages } from '../selectors';
import ChatMessageList from './ChatMessageList';
import SendMessageForm from './SendMessageForm';

type Props = {
   channel: string;
   participants: Participant[] | null;
   participantId?: string | null;
   participantColors: { [id: string]: string };
};

export default function Chat({ channel, participants, participantId, participantColors }: Props) {
   const messages = useSelector((state: RootState) => selectMessages(state, channel));
   const dispatch = useDispatch();

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
      <Box height="100%" display="flex" flexDirection="column">
         <Box flex={1}>
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
         </Box>
         <Box m={1}>
            <SendMessageForm channel={channel} isTyping={false} onSendMessage={handleSendMessage} />
         </Box>
      </Box>
   );
}
