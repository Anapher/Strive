import { Box, ClickAwayListener, Grow, IconButton, Paper, Popper, TextField, Typography } from '@material-ui/core';
import React, { useRef, useState } from 'react';
import SendIcon from '@material-ui/icons/Send';
import { useForm } from 'react-hook-form';
import EmojiEmotionsIcon from '@material-ui/icons/EmojiEmotions';
import EmojisPopper from './EmojisPopper';

type Props = {
   onSendMessage: (msg: string) => void;
};

export default function SendMessageForm({ onSendMessage }: Props) {
   const { register, handleSubmit, reset, setValue, watch } = useForm({ mode: 'onChange' });

   const message = watch('message');

   const [emojisPopperOpen, setEmojisPopperOpen] = useState(false);
   const emojisButtonRef = useRef(null);

   const handleCloseEmojis = () => setEmojisPopperOpen(false);
   const handleOpenEmojis = () => setEmojisPopperOpen(true);

   const inputRef = useRef<any | null>(null);

   const handleInsertEmoji = (s: string) => {
      setValue('message', message + s);
      handleCloseEmojis();
      inputRef.current?.focus();
   };

   return (
      <form
         noValidate
         onSubmit={handleSubmit(({ message }) => {
            onSendMessage(message);
            reset();
         })}
      >
         <TextField
            placeholder="Type your message..."
            autoComplete="off"
            fullWidth
            inputRef={(ref) => {
               register(ref);
               inputRef.current = ref;
            }}
            name="message"
         />
         <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
            <Typography>Vincent</Typography>
            <Box display="flex">
               <IconButton aria-label="emojis" ref={emojisButtonRef} onClick={handleOpenEmojis}>
                  <EmojiEmotionsIcon fontSize="small" />
               </IconButton>
               <IconButton aria-label="send" type="submit" disabled={!message}>
                  <SendIcon fontSize="small" />
               </IconButton>
            </Box>
         </Box>

         <Popper open={emojisPopperOpen} anchorEl={emojisButtonRef.current} transition placement="top-end">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps}>
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
      </form>
   );
}
