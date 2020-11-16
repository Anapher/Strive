import { makeStyles, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import { ChatMessageDto } from 'MyModels';
import React from 'react';
import { DateTime } from 'luxon';
import { ParticipantDto } from 'src/features/conference/types';
import emojiRegex from 'emoji-regex/RGI_Emoji';
import clsx from 'classnames';

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
}));

const onlyEmojisRegex = new RegExp('^(' + emojiRegex().toString().replace(/\/g$/, '') + '|\\s)+$');

type Props = {
   message?: ChatMessageDto;
   participants?: ParticipantDto[] | null;
};

export default function ChatMessage({ message, participants }: Props) {
   const classes = useStyles();
   const isEmoji = message && message.message.length <= 8 && onlyEmojisRegex.test(message.message);

   return (
      <li className={classes.root}>
         <div className={classes.metaData}>
            <Typography
               variant="caption"
               color="textSecondary"
               style={{ color: '#3498db', flex: 1, overflowX: 'hidden', textOverflow: 'ellipsis', marginRight: 8 }}
            >
               {message ? (
                  participants?.find((x) => x.participantId === message.participantId)?.displayName ??
                  message.participantId
               ) : (
                  <Skeleton />
               )}
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
