import { Box, makeStyles, Typography, useTheme } from '@material-ui/core';
import _ from 'lodash';
import { DateTime } from 'luxon';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import { selectParticipantList } from '../../selectors';
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
   const { t } = useTranslation();

   if (conferenceInfo.scheduledDate) {
      const date = DateTime.fromISO(conferenceInfo.scheduledDate);

      return (
         <Typography variant="h4" align="center">
            {t('conference_not_open.conference_scheduled_for')} <b>{date.toLocaleString(DateTime.DATETIME_FULL)}</b>
         </Typography>
      );
   }

   return (
      <div>
         <Typography variant="h4" align="center">
            {isModeratorJoined
               ? t('conference_not_open.waiting_for_moderator_to_open')
               : t('conference_not_open.waiting_for_moderator_to_join')}
         </Typography>
      </div>
   );
}

type Props = {
   conferenceInfo: SynchronizedConferenceInfo;
};

export default function ConferenceNotOpen({ conferenceInfo }: Props) {
   const classes = useStyles();
   const theme = useTheme();
   const { t } = useTranslation();
   const participants = useSelector(selectParticipantList);

   const isModeratorJoined = _.some(participants, (x) => conferenceInfo.moderators.includes(x.id));

   return (
      <ConferenceNotOpenLayout>
         <div className={classes.topContent}></div>
         <div>
            <ConferenceStatus conferenceInfo={conferenceInfo} isModeratorJoined={isModeratorJoined} />
         </div>
         <div className={classes.bottomContent}>
            {participants.length > 1 && (
               <Typography color="textSecondary" variant="caption" align="center" gutterBottom>
                  {t('conference_not_open.n_participants_waiting', { count: participants.length - 1 })}
                  {isModeratorJoined && (
                     <span style={{ color: theme.palette.secondary.main }}>
                        {' '}
                        {t('conference_not_open.moderator_joined')}
                     </span>
                  )}
               </Typography>
            )}
            <Box mt={6}>
               <Typography color="textSecondary" align="center">
                  {t('conference_not_open.you_dont_need_to_refresh')}
               </Typography>
            </Box>
         </div>
      </ConferenceNotOpenLayout>
   );
}
