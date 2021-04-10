import { Box, makeStyles, Typography, useTheme } from '@material-ui/core';
import _ from 'lodash';
import { DateTime } from 'luxon';
import React from 'react';
import { useSelector } from 'react-redux';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import { selectParticipants } from '../../selectors';
import ConferenceNotOpenLayout from './ConferenceNotOpenLayout';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      position: 'relative',
      display: 'flex',
      flexDirection: 'column',
   },
   container: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      flex: 1,
   },
   topContent: {
      flex: 1,
      display: 'flex',
      marginBottom: theme.spacing(2),
      flexDirection: 'column',
      justifyContent: 'flex-end',
   },
   bottomContent: {
      flex: 1,
      marginTop: theme.spacing(2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

type ConferenceStatusProps = {
   conferenceInfo: SynchronizedConferenceInfo;
   isModeratorJoined: boolean;
};

function ConferenceStatus({ conferenceInfo, isModeratorJoined }: ConferenceStatusProps) {
   if (conferenceInfo.scheduledDate) {
      const date = DateTime.fromISO(conferenceInfo.scheduledDate);
      return (
         <Typography variant="h4" align="center">
            The conference is scheduled for <b>{date.toLocaleString(DateTime.DATETIME_FULL)}</b>
         </Typography>
      );
   }

   return (
      <div>
         <Typography variant="h4" align="center">
            {isModeratorJoined
               ? 'Conference is not yet open. Waiting for moderator to open this conference...'
               : 'Conference is not yet open. Waiting for moderator to join...'}
         </Typography>
      </div>
   );
}

type Props = {
   conferenceInfo: SynchronizedConferenceInfo;
};

export default function ConferenceNotOpen({ conferenceInfo }: Props) {
   const classes = useStyles();
   const participants = useSelector(selectParticipants);

   const isModeratorJoined = _.some(participants, (x) => conferenceInfo.moderators.includes(x.id));

   const theme = useTheme();

   return (
      <ConferenceNotOpenLayout>
         <div className={classes.topContent}></div>
         <div>
            <ConferenceStatus conferenceInfo={conferenceInfo} isModeratorJoined={isModeratorJoined} />
         </div>
         <div className={classes.bottomContent}>
            {participants.length > 1 && (
               <Typography color="textSecondary" variant="caption" align="center" gutterBottom>
                  {participants.length - 1}{' '}
                  {participants.length > 2 ? 'participants are waiting.' : 'participant is waiting.'}
                  {isModeratorJoined && <span style={{ color: theme.palette.secondary.main }}> Moderator joined.</span>}
               </Typography>
            )}
            <Box mt={6}>
               <Typography color="textSecondary" align="center">
                  {"You don't need to refresh this page"}
               </Typography>
            </Box>
         </div>
      </ConferenceNotOpenLayout>
   );
}
