import { Badge, Fab, makeStyles, Zoom } from '@material-ui/core';
import KeyboardArrowDownIcon from '@material-ui/icons/KeyboardArrowDown';
import React, { useEffect, useRef, useState } from 'react';
import { Virtuoso, VirtuosoMethods } from 'react-virtuoso';
import { ParticipantDto } from 'src/features/conference/types';
import { getScrollbarWidth } from 'src/utils/browser-info';
import { ChatMessageDto } from '../types';
import ChatMessage from './ChatMessage';

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

   const [missedMessages, setMissedMessages] = useState(0);
   const [atBottom, setAtBottom] = useState(true);
   const virtuoso = useRef<VirtuosoMethods>(null);

   useEffect(() => {
      if (!atBottom) {
         setMissedMessages((x) => x + 1);
      }
   }, [chat]);

   useEffect(() => {
      if (atBottom) setMissedMessages(0);
   }, [atBottom]);

   const handleScrollToBottom = () => {
      if (chat) virtuoso.current?.scrollToIndex({ index: chat.length - 1, align: 'end', behavior: 'smooth' });
   };

   const generateItem = (index: number) => {
      return <ChatMessage participants={participants} message={chat === null ? undefined : chat[index]} />;
   };

   return (
      <div className={classes.root}>
         <Virtuoso
            ref={virtuoso}
            className={classes.list}
            totalCount={chat?.length ?? 3}
            item={generateItem}
            followOutput={true}
            atBottomStateChange={(val) => {
               setAtBottom(val);
            }}
         />
         <Zoom in={!atBottom}>
            <Fab
               color="primary"
               size="small"
               aria-label="scroll back down"
               className={classes.scrollDownFab}
               onClick={() => handleScrollToBottom()}
               style={{ right: 8 + getScrollbarWidth() }}
            >
               <Badge badgeContent={missedMessages} color="secondary">
                  <KeyboardArrowDownIcon />
               </Badge>
            </Fab>
         </Zoom>
      </div>
   );
}
