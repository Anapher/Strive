import { Box, IconButton, TextField, Typography } from '@material-ui/core';
import React from 'react';
import SendIcon from '@material-ui/icons/Send';
import { useForm } from 'react-hook-form';

type Props = {
   onSendMessage: (msg: string) => void;
};

export default function SendMessageForm({ onSendMessage }: Props) {
   const { register, handleSubmit, reset, formState } = useForm({ mode: 'onChange' });

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
            inputRef={register({ required: true })}
            name="message"
         />
         <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
            <Typography>Vincent</Typography>
            <IconButton aria-label="send" type="submit" disabled={!formState.isValid || !formState.isDirty}>
               <SendIcon fontSize="small" />
            </IconButton>
         </Box>
      </form>
   );
}
