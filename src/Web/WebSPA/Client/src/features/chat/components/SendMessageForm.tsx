import { Box, ClickAwayListener, Grow, IconButton, Paper, Popper } from '@material-ui/core';
import EmojiEmotionsIcon from '@material-ui/icons/EmojiEmotions';
import SendIcon from '@material-ui/icons/Send';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useDispatch } from 'react-redux';
import { setUserTyping } from 'src/core-hub';
import { ChatMessageOptions, SendChatMessageDto } from 'src/core-hub.types';
import useBooleanThrottle from '../useBooleanThrottle';
import ChatMessageInput from './ChatMessageInput';
import EmojisPopper from './EmojisPopper';
import SendMessageOptions from './SendMessageOptions';

type Props = {
   onSendMessage: (msg: SendChatMessageDto) => void;
   channel: string;
   isTyping: boolean;
};

export default function SendMessageForm({ onSendMessage, isTyping, channel }: Props) {
   const dispatch = useDispatch();

   const [message, setMessage] = useState('');
   const [options, setOptions] = useState<ChatMessageOptions>({ isAnonymous: false, isAnnouncement: false });

   const inputRef = useRef<HTMLInputElement | null>(null);
   const focusMessageInput = () => inputRef.current?.focus();

   const [emojisPopperOpen, setEmojisPopperOpen] = useState(false);
   const emojisButtonRef = useRef(null);

   const handleCloseEmojis = () => setEmojisPopperOpen(false);
   const handleOpenEmojis = () => setEmojisPopperOpen(true);

   const handleInsertEmoji = (s: string) => {
      setMessage((msg) => msg + s);
      handleCloseEmojis();
      focusMessageInput();
   };

   const watchUserTyping = !options.isAnonymous;

   useEffect(() => {
      if (inputRef.current) {
         inputRef.current.focus();
      }
   }, [inputRef.current]);

   useEffect(() => {
      // display as not typing if the participant changed to anonymous
      if (isTyping && !watchUserTyping) {
         dispatch(setUserTyping({ isTyping: false, channel }));
      }
   }, [watchUserTyping, channel]);

   const applyParticipantIsTyping = useCallback(
      (newValue: boolean) => {
         if (newValue) {
            if (!inputRef.current?.value) return;
         }

         dispatch(setUserTyping({ isTyping: newValue, channel }));
      },
      [dispatch, inputRef.current, channel],
   );

   const handleSetIsTyping = useBooleanThrottle(isTyping, applyParticipantIsTyping);

   const handleChangeMessage = (s: string) => setMessage(s);

   const handleSubmit = () => {
      if (message) {
         onSendMessage({ message, options, channel });
         setMessage('');
      }
   };

   return (
      <div>
         <Box m={1}>
            <ChatMessageInput
               onSubmit={handleSubmit}
               ref={inputRef}
               onChangeIsTyping={handleSetIsTyping}
               value={message}
               onChange={handleChangeMessage}
               watchUserTyping={watchUserTyping}
            />
         </Box>
         <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
            <Box>
               <SendMessageOptions value={options} onChange={setOptions} channel={channel} />
            </Box>
            <Box display="flex">
               <IconButton aria-label="emojis" ref={emojisButtonRef} onClick={handleOpenEmojis}>
                  <EmojiEmotionsIcon fontSize="small" />
               </IconButton>
               <IconButton aria-label="send" onClick={handleSubmit} disabled={!message}>
                  <SendIcon fontSize="small" />
               </IconButton>
            </Box>
         </Box>

         <Popper open={emojisPopperOpen} anchorEl={emojisButtonRef.current} transition placement="top-end">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps} style={{ transformOrigin: 'right bottom' }}>
                  <Paper>
                     <ClickAwayListener onClickAway={handleCloseEmojis}>
                        <Box p={1}>
                           <EmojisPopper onClose={handleCloseEmojis} onEmojiSelected={handleInsertEmoji} />
                        </Box>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </div>
   );
}
