import React, { useMemo } from 'react';
import { IncognitoCircle, Bullhorn } from 'mdi-material-ui';
import { IconButton, makeStyles, Tooltip } from '@material-ui/core';
import { ChatMessageOptions } from 'src/core-hub.types';
import usePermission from 'src/hooks/usePermission';
import { CHAT_CAN_SEND_ANNOUNCEMENT, CHAT_CAN_SEND_ANONYMOUSLY } from 'src/permissions';
import { decode } from '../channel-serializer';
import CloseIcon from '@material-ui/icons/Close';
import { useDispatch } from 'react-redux';
import { closePrivateChat } from '../reducer';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
   },
   iconSelected: {
      color: theme.palette.text.primary,
   },
   iconUnseleted: {
      color: theme.palette.text.disabled,
   },
}));

type Props = {
   channel: string;

   value: ChatMessageOptions;
   onChange: (value: ChatMessageOptions) => void;
};

export default function SendMessageOptions({ value, onChange, channel }: Props) {
   const classes = useStyles();

   const dispatch = useDispatch();

   const selectIconButtonClass = (enabled: boolean) => (enabled ? classes.iconSelected : classes.iconUnseleted);

   const handleToggleAnonymous = () => onChange({ ...value, isAnonymous: !value.isAnonymous });
   const handleToggleHighlighted = () => onChange({ ...value, isAnnouncement: !value.isAnnouncement });

   const handleClosePrivateChat = () => dispatch(closePrivateChat(channel));

   const canSendAnonymousMessage = usePermission(CHAT_CAN_SEND_ANONYMOUSLY);
   const canSendAnnouncement = usePermission(CHAT_CAN_SEND_ANNOUNCEMENT);

   const isPrivateChat = useMemo(() => decode(channel).type === 'private', [channel]);

   return (
      <div>
         {isPrivateChat && (
            <Tooltip title="Close private chat">
               <IconButton aria-label="close private chat" onClick={handleClosePrivateChat} color="secondary">
                  <CloseIcon />
               </IconButton>
            </Tooltip>
         )}
         {canSendAnonymousMessage && !isPrivateChat && (
            <Tooltip title="Send the message anonymously">
               <IconButton
                  aria-label="anonymous"
                  className={selectIconButtonClass(value.isAnonymous)}
                  onClick={handleToggleAnonymous}
               >
                  <IncognitoCircle />
               </IconButton>
            </Tooltip>
         )}
         {canSendAnnouncement && (
            <Tooltip title="Send the message as announcement">
               <IconButton
                  aria-label="highlighted"
                  className={selectIconButtonClass(value.isAnnouncement)}
                  onClick={handleToggleHighlighted}
               >
                  <Bullhorn />
               </IconButton>
            </Tooltip>
         )}
      </div>
   );
}
