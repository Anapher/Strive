import { TextField } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';

type Props = {
   value: string;
   watchUserTyping: boolean;

   onChange: (s: string) => void;
   onChangeIsTyping: (isTyping: boolean) => void;
   onSubmit: () => void;
};

export default React.forwardRef<HTMLInputElement, Props>(function ChatMessageInput(
   { value, onChange, watchUserTyping, onChangeIsTyping, onSubmit },
   ref,
) {
   const { t } = useTranslation();
   const handleTextFieldKeyPress = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (event.key === 'Enter' && !event.shiftKey) {
         onSubmit();
         event.preventDefault(); // Prevents the addition of a new line in the text field (not needed in a lot of cases)
      }
   };

   const handleTextFieldKeyUp = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (!watchUserTyping) return; // only show if the participant is typing if sent to all

      const newValue = (event.target as any).value;
      onChangeIsTyping(!!newValue);
   };

   return (
      <TextField
         name="message"
         multiline
         rowsMax={3}
         placeholder={t('conference.chat.type_your_message')}
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
