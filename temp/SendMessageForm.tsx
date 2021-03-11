import {
   Box,
   ClickAwayListener,
   Grow,
   IconButton,
   ListSubheader,
   MenuItem,
   Paper,
   Popper,
   Select,
   TextField,
} from '@material-ui/core';
import EmojiEmotionsIcon from '@material-ui/icons/EmojiEmotions';
import SendIcon from '@material-ui/icons/Send';
import _ from 'lodash';
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import CompactInput from 'src/components/CompactInput';
import { setUserTyping } from 'src/core-hub';
import { SendChatMessageDto, SendingMode } from 'src/core-hub.types';
import { selectOtherParticipants } from 'src/features/conference/selectors';
import usePermission from 'src/hooks/usePermission';
import { CHAT_CAN_SEND_ANONYMOUS_MESSAGE, CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE } from 'src/permissions';
import { RootState } from 'src/store';
import { setSendingMode } from '../reducer';
import EmojisPopper from './EmojisPopper';

type Props = {
   onSendMessage: (msg: SendChatMessageDto) => void;
   isTyping: boolean;
};

type SendMessageFormType = {
   message: string;
};

const sendingModeToString = (s: SendingMode | null) => {
   if (s === null) return 'all';
   if (s.type === 'anonymously') return 'anonymously';
   return `to:${s.to.participantId}`;
};

const stringToSendingMode: (s: string) => SendingMode | null = (s) => {
   if (s?.startsWith('to:')) {
      return { type: 'privately', to: { participantId: s.substring(3) } };
   } else if (s === 'anonymously') return { type: 'anonymously' };

   return null;
};

export default function SendMessageForm({ onSendMessage, isTyping }: Props) {
   const { register, handleSubmit, setValue, watch } = useForm<SendMessageFormType>({
      mode: 'onChange',
   });

   const sendTo = useSelector((state: RootState) => state.chat.sendingMode);

   const canSendPrivateMsg = usePermission(CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE);
   const canSendAnonymousMsg = usePermission(CHAT_CAN_SEND_ANONYMOUS_MESSAGE);

   const message = watch('message');
   const dispatch = useDispatch();

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

   const handleKeyPressNotEmpty = useCallback(
      _.throttle(() => {
         if (inputRef.current && inputRef.current.value) dispatch(setUserTyping(true));
      }, 10000),
      [dispatch, inputRef.current, isTyping],
   );

   useEffect(() => {
      if (inputRef.current) {
         inputRef.current.focus();
      }

      // display as not typing if the participant changed the sendTo to private or anonymous
      if (isTyping && sendTo !== null) {
         dispatch(setUserTyping(false));
      }
   }, [inputRef.current, sendTo]);

   const handleTextFieldKeyPress = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (event.key === 'Enter' && !event.shiftKey) {
         (event.target as any).form.dispatchEvent(new Event('submit', { cancelable: true }));
         event.preventDefault(); // Prevents the addition of a new line in the text field (not needed in a lot of cases)
      }
   };

   const handleTextFieldKeyUp = (event: React.KeyboardEvent<HTMLDivElement>) => {
      if (sendTo !== null) return; // only show if the participant is typing if sent to all

      const newValue = (event.target as any).value;
      if (newValue) {
         handleKeyPressNotEmpty();
      } else {
         if (isTyping) {
            dispatch(setUserTyping(false));
         }
      }
   };

   const handleChangeSendTo = (event: React.ChangeEvent<{ value: unknown }>) => {
      if (event.target.value) {
         dispatch(setSendingMode(stringToSendingMode(event.target.value as string)));
      }
   };

   const participants = useSelector(selectOtherParticipants);
   const sortedParticipants = useMemo(() => _.sortBy(participants, (x) => x.displayName), [participants]);

   return (
      <form
         noValidate
         onSubmit={handleSubmit(({ message }) => {
            if (message) {
               onSendMessage({ message, mode: sendTo });
               setValue('message', '');
               dispatch(setUserTyping(false));
            }
         })}
      >
         <TextField
            multiline
            rowsMax={3}
            placeholder="Type your message..."
            autoComplete="off"
            fullWidth
            onKeyPress={handleTextFieldKeyPress}
            onKeyUp={handleTextFieldKeyUp}
            inputRef={(ref) => {
               register(ref);
               inputRef.current = ref;
            }}
            name="message"
         />
         <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center">
            <Select
               onChange={handleChangeSendTo}
               style={{ flex: 1 }}
               variant="outlined"
               input={<CompactInput />}
               fullWidth
               value={sendingModeToString(sendTo)}
            >
               <MenuItem value="all">Send to all</MenuItem>
               {canSendAnonymousMsg && <MenuItem value="anonymously">Send to all anonymously</MenuItem>}
               {canSendPrivateMsg &&
                  participants &&
                  participants.length > 0 &&
                  [<ListSubheader key="separator">Send privately to</ListSubheader>].concat(
                     sortedParticipants.map(({ id, displayName }) => (
                        <MenuItem key={id} value={`to:${id}`}>
                           {displayName}
                        </MenuItem>
                     )),
                  )}
            </Select>

            <Box display="flex" ml={1}>
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
