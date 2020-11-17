import { Badge, Fab, List, makeStyles, useScrollTrigger, Zoom } from '@material-ui/core';
import { ChatMessageDto } from '../types';
import React, { useEffect, useRef, useState } from 'react';
import { ParticipantDto } from 'src/features/conference/types';
import ChatMessage from './ChatMessage';
import KeyboardArrowDownIcon from '@material-ui/icons/KeyboardArrowDown';
import useBottomScrollTrigger from 'src/hooks/useBottomScrollTrigger';

const useStyles = makeStyles({
   root: {
      height: '100%',
      position: 'relative',
   },
   list: {
      overflowY: 'scroll',
      height: '100%',
   },
   scrollDownFab: {
      position: 'absolute',
      bottom: 8,
   },
});

type Props = {
   chat: ChatMessageDto[] | null;
   participants: ParticipantDto[] | null;
};

export default function ChatMessageList({ chat, participants }: Props) {
   const classes = useStyles();
   const listRef = useRef<HTMLOListElement>(null);
   const bottomAnchor = useRef<HTMLDivElement>(null);

   const [missedMessages, setMissedMessages] = useState(0);

   const trigger = useBottomScrollTrigger({
      target: listRef.current ?? undefined,
      disableHysteresis: true,
      threshold: 25,
   });

   const handleScrollToBottom = (behavior: 'auto' | 'smooth' = 'smooth') => {
      if (bottomAnchor.current) {
         bottomAnchor.current.scrollIntoView({ behavior, block: 'center' });
      }
   };

   useEffect(() => {
      if (listRef.current) {
         listRef.current.scrollTop = listRef.current.scrollHeight - listRef.current.clientHeight;
      }
   }, [listRef.current]);

   useEffect(() => {
      if (!trigger) {
         handleScrollToBottom('auto');
      } else {
         setMissedMessages((x) => x + 1);
      }
   }, [chat]);

   useEffect(() => {
      if (!trigger) setMissedMessages(0);
   }, [trigger]);

   let scrollbarWidth = 0;
   if (listRef.current && bottomAnchor.current) {
      scrollbarWidth = listRef.current.offsetWidth - bottomAnchor.current.offsetWidth;
   }

   return (
      <div className={classes.root}>
         <List className={classes.list} ref={listRef}>
            {chat !== null ? (
               chat.map((x) => <ChatMessage key={x.messageId} message={x} participants={participants} />)
            ) : (
               <>
                  <ChatMessage />
                  <ChatMessage />
                  <ChatMessage />
               </>
            )}
            <div ref={bottomAnchor} />
         </List>
         <Zoom in={trigger}>
            <Fab
               color="primary"
               size="small"
               aria-label="scroll back down"
               className={classes.scrollDownFab}
               onClick={() => handleScrollToBottom()}
               style={{ right: scrollbarWidth + 8 }}
            >
               <Badge badgeContent={missedMessages} color="secondary">
                  <KeyboardArrowDownIcon />
               </Badge>
            </Fab>
         </Zoom>
      </div>
   );
}
