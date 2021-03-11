import { TextField } from '@material-ui/core';
import _ from 'lodash';
import React, { useCallback } from 'react';

type Props = {
   value: string;
   onChange: (s: string) => void;

   watchUserTyping: boolean;
   isTyping: boolean;
   onChangeIsTyping: (isTyping: boolean) => void;

   onSubmit: () => void;
};

export default React.forwardRef<HTMLInputElement, Props>(function ChatMessageInput(
   { value, onChange, watchUserTyping, isTyping, onChangeIsTyping, onSubmit },
   ref,
) {
   const handleTextFieldKeyPress = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (event.key === 'Enter' && !event.shiftKey) {
         onSubmit();
         event.preventDefault(); // Prevents the addition of a new line in the text field (not needed in a lot of cases)
      }
   };

   const handleTextFieldKeyUp = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (!watchUserTyping) return; // only show if the participant is typing if sent to all

      const newValue = (event.target as any).value;
      if (newValue) {
         handleKeyPressNotEmpty();
      } else {
         if (isTyping) {
            onChangeIsTyping(false);
         }
      }
   };

   const handleKeyPressNotEmpty = useCallback(
      _.throttle(() => {
         onChangeIsTyping(true);
      }, 10000),
      [onChangeIsTyping],
   );

   return (
      <TextField
         name="message"
         multiline
         rowsMax={3}
         placeholder="Type your message..."
         autoComplete="off"
         fullWidth
         onKeyPress={handleTextFieldKeyPress}
         onKeyUp={handleTextFieldKeyUp}
         value={value}
         onChange={(ev) => onChange(ev.target.value)}
         inputRef={ref}
      />
   );
});
