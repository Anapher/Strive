import { makeStyles } from '@material-ui/core';
import { ChatMessageDto } from 'MyModels';
import React from 'react';
import { ParticipantDto } from 'src/features/conference/types';
import ChatMessage from './ChatMessage';

const useStyles = makeStyles({
   root: {
      height: '100%',
      overflowY: 'scroll',
   },
});

type Props = {
   chat: ChatMessageDto[] | null;
   participants: ParticipantDto[] | null;
};

export default function ChatMessageList({ chat, participants }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         {chat !== null ? (
            chat.map((x) => <ChatMessage key={x.messageId} message={x} participants={participants} />)
         ) : (
            <>
               <ChatMessage />
               <ChatMessage />
               <ChatMessage />
            </>
         )}
      </div>
   );
}
