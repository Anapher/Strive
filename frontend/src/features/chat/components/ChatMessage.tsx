import { makeStyles, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React, { useMemo } from 'react';
import { DateTime } from 'luxon';
import { ParticipantDto } from 'src/features/conference/types';
import emojiRegex from 'emoji-regex/RGI_Emoji';
import clsx from 'classnames';
import { ChatMessageDto } from '../types';
import { hashCode, numberToColor } from '../color-utils';

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
   messageText: {
      whiteSpace: 'pre-wrap',
      wordBreak: 'break-all',
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
   privateBadge: {
      backgroundColor: theme.palette.primary.main,
      color: theme.palette.primary.contrastText,
      padding: '2px 4px',
      borderRadius: theme.shape.borderRadius,
      marginRight: 8,
   },
   disconnectedText: {
      color: theme.palette.text.secondary,
      marginLeft: theme.spacing(1),
   },
}));

const onlyEmojisRegex = new RegExp('^(' + emojiRegex().toString().replace(/\/g$/, '') + '|\\s)+$');

type Props = {
   message?: ChatMessageDto;
   participants?: ParticipantDto[] | null;
   participantId?: string;
   participantColors: { [id: string]: string };
};

export default function ChatMessage({ message, participants, participantId, participantColors }: Props) {
   const classes = useStyles();
   const isEmoji = message && message.message.length <= 8 && onlyEmojisRegex.test(message.message);
   const sender = message?.from && participants?.find((x) => x.participantId === message.from?.participantId);

   const isAnonymous = message && !message.from;
   const isDisconnected = message?.from && !sender;
   const isFromMe = message?.from?.participantId === participantId;

   const participantColor = useMemo(
      () =>
         message?.from &&
         (participantColors[message.from.participantId] ?? numberToColor(hashCode(message.from.participantId))), // numberToColor for participants that just disconnected
      [message?.from?.participantId],
   );

   return (
      <li className={classes.root} style={{ opacity: isDisconnected ? 0.8 : undefined }}>
         <div className={classes.metaData}>
            <Typography
               variant="caption"
               style={{ color: participantColor }}
               className={clsx(classes.senderText, {
                  [classes.senderTextAnonymous]: isAnonymous,
                  [classes.senderTextPrivate]: message?.mode?.type === 'privately',
               })}
            >
               {message?.mode?.type === 'privately' && (
                  <span className={classes.privateBadge}>
                     Private{isFromMe && ` -> ${message.mode.to.displayName}`}
                  </span>
               )}
               {renderSender(message, sender, isAnonymous)}
               {isDisconnected && <span className={classes.disconnectedText}>(Disconnected)</span>}
            </Typography>
            <Typography variant="caption" color="textSecondary">
               {message ? (
                  DateTime.fromISO(message.timestamp).toLocaleString(DateTime.TIME_24_SIMPLE)
               ) : (
                  <Skeleton width={36} />
               )}
            </Typography>
         </div>
         <Typography variant="body1" className={clsx(classes.messageText, isEmoji && classes.emojiText)}>
            {message ? message.message : <Skeleton />}
         </Typography>
      </li>
   );
}

function renderSender(message?: ChatMessageDto, sender?: ParticipantDto, isAnonymous?: boolean) {
   if (message?.from) {
      return sender?.displayName ?? message.from.displayName ?? message.from.participantId;
   }

   if (isAnonymous) {
      return 'Anonymous';
   }

   return <Skeleton />;
}
