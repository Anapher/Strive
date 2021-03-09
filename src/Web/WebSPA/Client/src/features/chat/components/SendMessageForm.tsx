import { Box, ClickAwayListener, Grow, IconButton, Paper, Popper } from '@material-ui/core';
import EmojiEmotionsIcon from '@material-ui/icons/EmojiEmotions';
import SendIcon from '@material-ui/icons/Send';
import React, { useEffect, useRef, useState } from 'react';
import { useDispatch } from 'react-redux';
import { setUserTyping } from 'src/core-hub';
import { ChatMessageOptions, SendChatMessageDto } from 'src/core-hub.types';
import ChatMessageInput from './ChatMessageInput';
import EmojisPopper from './EmojisPopper';

type Props = {
   onSendMessage: (msg: SendChatMessageDto) => void;
   channel: string;
   isTyping: boolean;
};

export default function SendMessageForm({ onSendMessage, isTyping, channel }: Props) {
   const dispatch = useDispatch();

   const [message, setMessage] = useState('');
   const [options, setOptions] = useState<ChatMessageOptions>({ isAnonymous: false, isHighlighted: false });

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
      console.log(inputRef.current);
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
         dispatch(setUserTyping(false));
      }
   }, [watchUserTyping]);

   const handleOnChangeIsTyping = (isTyping: boolean) => {
      if (isTyping) {
         if (!inputRef.current?.value) return;
      }

      dispatch(setUserTyping(false));
   };

   const handleChangeMessage = (s: string) => setMessage(s);

   const handleSubmit = () => {
      if (message) {
         onSendMessage({ message, options, channel });
         setMessage('');
         dispatch(setUserTyping(false));
      }
   };

   return (
      <div>
         <ChatMessageInput
            onSubmit={handleSubmit}
            ref={inputRef}
            onChangeIsTyping={handleOnChangeIsTyping}
            isTyping={isTyping}
            value={message}
            onChange={handleChangeMessage}
            watchUserTyping={watchUserTyping}
         />
         <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
            <Box display="flex" ml={1}>
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
