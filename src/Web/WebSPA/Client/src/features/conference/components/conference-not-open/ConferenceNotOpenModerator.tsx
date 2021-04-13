import { Button, ButtonGroup, makeStyles, Typography } from '@material-ui/core';
import SettingsIcon from '@material-ui/icons/Settings';
import { DateTime } from 'luxon';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { useParams } from 'react-router-dom';
import * as coreHub from 'src/core-hub';
import { openDialogToPatchAsync } from 'src/features/create-conference/reducer';
import usePermission from 'src/hooks/usePermission';
import { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/permissions';
import { ConferenceRouteParams } from 'src/routes/types';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import { selectParticipants } from '../../selectors';
import ConferenceNotOpenLayout from './ConferenceNotOpenLayout';

const useStyles = makeStyles((theme) => ({
   topContent: {
      flex: 1,
      display: 'flex',
      marginBottom: theme.spacing(2),
      flexDirection: 'column',
      alignItems: 'center',
   },
   bottomContent: {
      flex: 1,
      marginTop: theme.spacing(2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
   scheduledForContainer: {
      flex: 1,
      display: 'flex',
      alignItems: 'center',
   },
   scheduledDateText: {
      color: theme.palette.text.primary,
   },
   fill: { flex: 1 },
}));

type Props = {
   conferenceInfo: SynchronizedConferenceInfo;
};

export default function ConferenceNotOpenModerator({ conferenceInfo }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const { id: conferenceId } = useParams<ConferenceRouteParams>();

   const participants = useSelector(selectParticipants);

   const canOpen = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleOpenConference = () => dispatch(coreHub.openConference());
   const handlePatchConference = () => {
      if (conferenceId) dispatch(openDialogToPatchAsync(conferenceId));
   };

   return (
      <ConferenceNotOpenLayout>
         <div className={classes.topContent}>
            <div className={classes.scheduledForContainer}>
               {conferenceInfo.scheduledDate && (
                  <Typography color="textSecondary">
                     {t('conference_not_open.conference_scheduled_for') + ' '}
                     <span className={classes.scheduledDateText}>
                        {DateTime.fromISO(conferenceInfo.scheduledDate).toLocaleString(DateTime.DATETIME_FULL)}
                     </span>
                  </Typography>
               )}
            </div>
            <Typography gutterBottom>{t('conference_not_open.you_are_moderator')}</Typography>
         </div>
         <ButtonGroup variant="contained" color="primary">
            <Button onClick={handleOpenConference} disabled={!canOpen}>
               {t('conference_not_open.open_conference')}
            </Button>
            <Button onClick={handlePatchConference} aria-label={t('conference_not_open.change_conference_settings')}>
               <SettingsIcon />
            </Button>
         </ButtonGroup>

         <div className={classes.bottomContent}>
            {participants.length > 1 && (
               <Typography color="textSecondary" variant="caption" align="center" gutterBottom>
                  {t('conference_not_open.n_participants_waiting', { count: participants.length - 1 })}
               </Typography>
            )}
            <div className={classes.fill} />
         </div>
      </ConferenceNotOpenLayout>
   );
}
