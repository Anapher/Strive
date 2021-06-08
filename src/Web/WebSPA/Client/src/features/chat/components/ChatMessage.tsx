import { makeStyles, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import clsx from 'classnames';
import emojiRegex from 'emoji-regex/RGI_Emoji';
import { Options } from 'linkifyjs';
import Linkify from 'linkifyjs/react';
import { DateTime } from 'luxon';
import { Bullhorn } from 'mdi-material-ui';
import { useMemo, useRef } from 'react';
import { useSelector } from 'react-redux';
import { ChatMessageDto } from 'src/core-hub.types';
import { selectParticipant } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { getParticipantColor } from '../utils';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',

      paddingLeft: theme.spacing(2),
      paddingRight: theme.spacing(2),
      paddingTop: 4,
      paddingBottom: 4,
   },
   metaData: {
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'space-between',
   },
   nameWithIcons: {
      display: 'flex',
      alignItems: 'flex-end',
   },
   messageText: {
      whiteSpace: 'pre-wrap',
      wordBreak: 'break-word',
      fontSize: 14,
   },
   emojiText: {
      fontSize: 20,
   },
   senderText: {
      flex: 1,
      overflowX: 'hidden',
      textOverflow: 'ellipsis',
      marginRight: theme.spacing(1),
   },
   senderTextAnonymous: {
      color: theme.palette.text.secondary,
   },
   senderTextPrivate: {
      color: theme.palette.primary.main,
   },
   disconnectedText: {
      color: theme.palette.text.secondary,
      marginLeft: theme.spacing(1),
   },
   anchor: {
      color: theme.palette.primary.light,
   },
}));

const onlyEmojisRegex = new RegExp('^(' + emojiRegex().toString().replace(/\/g$/, '') + '|\\s)+$');

type Props = {
   message?: ChatMessageDto;
   participantId?: string | null;
   participantColors: { [id: string]: string };
};

export default function ChatMessage({ message, participantColors }: Props) {
   const classes = useStyles();
   const isEmoji = message && message.message.length <= 8 && onlyEmojisRegex.test(message.message);
   const sender = useSelector((state: RootState) => selectParticipant(state, message?.sender?.participantId));

   const isAnonymous = message && !message.sender;
   const isDisconnected = message?.sender && !sender;

   const linifyOptions = useRef<Options>({ className: classes.anchor, target: '_blank' });

   const participantColor = useMemo(
      () =>
         message?.sender &&
         (participantColors[message.sender.participantId] ?? getParticipantColor(message.sender.participantId)), // numberToColor for participants that just disconnected
      [message?.sender?.participantId],
   );

   return (
      <li className={classes.root} style={{ opacity: isDisconnected ? 0.8 : undefined }}>
         <div className={classes.metaData}>
            <div className={classes.nameWithIcons}>
               <Typography
                  variant="caption"
                  style={{ color: participantColor }}
                  className={clsx(classes.senderText, {
                     [classes.senderTextAnonymous]: isAnonymous,
                  })}
               >
                  {renderSender(message, sender, isAnonymous)}
                  {isDisconnected && <span className={classes.disconnectedText}>(Disconnected)</span>}
               </Typography>
               {message?.options.isAnnouncement && <Bullhorn style={{ width: 16 }} />}
            </div>
            <Typography variant="caption" color="textSecondary">
               {message ? (
                  DateTime.fromISO(message.timestamp).toLocaleString(DateTime.TIME_24_SIMPLE)
               ) : (
                  <Skeleton width={36} />
               )}
            </Typography>
         </div>
         <Typography variant="body1" className={clsx(classes.messageText, isEmoji && classes.emojiText)}>
            <Linkify options={linifyOptions.current}>{message ? message.message : <Skeleton />}</Linkify>
         </Typography>
      </li>
   );
}

function renderSender(message?: ChatMessageDto, sender?: Participant, isAnonymous?: boolean) {
   if (message?.sender) {
      return sender?.displayName ?? message.sender.meta.displayName;
   }

   if (isAnonymous) {
      return 'Anonymous';
   }

   return <Skeleton />;
}
