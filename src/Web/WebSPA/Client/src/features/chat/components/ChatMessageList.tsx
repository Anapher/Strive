import { Badge, Box, Button, Fab, List, makeStyles, Typography, Zoom } from '@material-ui/core';
import KeyboardArrowDownIcon from '@material-ui/icons/KeyboardArrowDown';
import _ from 'lodash';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { DomainError } from 'src/communication-types';
import { ChatMessageDto } from 'src/core-hub.types';
import useBottomScrollTrigger from 'src/hooks/useBottomScrollTrigger';
import { getScrollbarWidth } from 'src/utils/browser-info';
import ChatMessage from './ChatMessage';

const useStyles = makeStyles({
   root: {
      position: 'relative',
      flex: 1,
      overflowY: 'hidden',
   },
   list: {
      overflowY: 'scroll',
      display: 'flex',
      flexDirection: 'column-reverse',
      height: '100%',
   },
   scrollDownFab: {
      position: 'absolute',
      bottom: 8,
   },
});

type Props = {
   messages: ChatMessageDto[] | undefined;
   participantId?: string | null;
   participantColors: { [id: string]: string };
   error?: DomainError | null;
   onRetry: () => void;
};

export default function ChatMessageList({ messages, participantId, participantColors, error, onRetry }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const listRef = useRef<HTMLOListElement>(null);
   const bottomAnchor = useRef<HTMLDivElement>(null);

   const [missedMessages, setMissedMessages] = useState(0);

   const atBottom = useBottomScrollTrigger({
      target: listRef.current ?? undefined,
      disableHysteresis: true,
      threshold: 25,
   });

   useEffect(() => {
      if (atBottom) setMissedMessages(0);
   }, [atBottom]);

   useEffect(() => {
      if (!atBottom) {
         setMissedMessages((x) => x + 1);
      }
   }, [messages]);

   const handleScrollToBottom = (behavior: 'auto' | 'smooth' = 'smooth') => {
      if (bottomAnchor.current) {
         bottomAnchor.current.scrollIntoView({ behavior, block: 'center' });
      }
   };

   return (
      <div className={classes.root}>
         <List className={classes.list} ref={listRef} id="chat-message-list">
            <div ref={bottomAnchor} />
            {messages == null || messages == undefined || error ? (
               error ? (
                  <Box m={2} display="flex" flexDirection="column" alignItems="center">
                     <Typography color="error" gutterBottom align="center">
                        {t('conference.chat.error_fetch_chat', { error })}
                     </Typography>
                     <Button onClick={onRetry} variant="contained">
                        {t('common:retry')}
                     </Button>
                  </Box>
               ) : (
                  Array.from({ length: 8 }).map((_, i) => <ChatMessage key={i} participantColors={participantColors} />)
               )
            ) : (
               _([...messages])
                  .reverse()
                  .map((message) => (
                     <ChatMessage
                        key={message.id}
                        participantColors={participantColors}
                        message={message}
                        participantId={participantId}
                     />
                  ))
                  .value()
            )}
         </List>
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
