import { Box, ClickAwayListener, Grow, IconButton, Paper, Popper, TextField, Typography } from '@material-ui/core';
import EmojiEmotionsIcon from '@material-ui/icons/EmojiEmotions';
import SendIcon from '@material-ui/icons/Send';
import React, { useRef, useState } from 'react';
import { useForm } from 'react-hook-form';
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

   const inputRef = useRef<HTMLInputElement | null>(null);

   const handleInsertEmoji = (s: string) => {
      setValue('message', message + s);
      handleCloseEmojis();
      inputRef.current?.focus();
   };

   const handleTextFieldKeyPress = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (event.key === 'Enter' && !event.shiftKey) {
         (event.target as any).form.dispatchEvent(new Event('submit', { cancelable: true }));
         event.preventDefault(); // Prevents the addition of a new line in the text field (not needed in a lot of cases)
      }
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
            multiline
            rowsMax={3}
            placeholder="Type your message..."
            autoComplete="off"
            fullWidth
            onKeyPress={handleTextFieldKeyPress}
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
      </form>
   );
}
