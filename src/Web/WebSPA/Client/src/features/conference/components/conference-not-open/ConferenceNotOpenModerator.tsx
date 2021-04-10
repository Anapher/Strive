import { Button, ButtonGroup, makeStyles, Typography, useTheme } from '@material-ui/core';
import _ from 'lodash';
import { DateTime } from 'luxon';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useParams } from 'react-router-dom';
import * as coreHub from 'src/core-hub';
import { openDialogToPatchAsync } from 'src/features/create-conference/reducer';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/permissions';
import { ConferenceRouteParams } from 'src/routes/types';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import { selectParticipants } from '../../selectors';
import ConferenceNotOpenLayout from './ConferenceNotOpenLayout';
import SettingsIcon from '@material-ui/icons/Settings';

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
   const participants = useSelector(selectParticipants);
   const myId = useMyParticipantId();

   const { id: conferenceId } = useParams<ConferenceRouteParams>();

   const isUserModerator = conferenceInfo.moderators.includes(myId);
   const isModeratorJoined = _.some(participants, (x) => conferenceInfo.moderators.includes(x.id));

   const theme = useTheme();

   const dispatch = useDispatch();
   const handleOpenConference = () => dispatch(coreHub.openConference());
   const canOpen = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);

   const handlePatchConference = () => {
      if (conferenceId) dispatch(openDialogToPatchAsync(conferenceId));
   };

   return (
      <ConferenceNotOpenLayout>
         <div className={classes.topContent}>
            <div className={classes.scheduledForContainer}>
               {conferenceInfo.scheduledDate && (
                  <Typography color="textSecondary">
                     Conference is scheduled for{' '}
                     <span className={classes.scheduledDateText}>
                        {DateTime.fromISO(conferenceInfo.scheduledDate).toLocaleString(DateTime.DATETIME_FULL)}
                     </span>
                  </Typography>
               )}
            </div>
            <Typography gutterBottom>You are a moderator of this conference.</Typography>
         </div>
         <ButtonGroup variant="contained" color="primary">
            <Button onClick={handleOpenConference} disabled={!canOpen}>
               Open conference
            </Button>
            <Button onClick={handlePatchConference}>
               <SettingsIcon />
            </Button>
         </ButtonGroup>

         <div className={classes.bottomContent}>
            {participants.length > 1 && (
               <Typography color="textSecondary" variant="caption" align="center" gutterBottom>
                  {participants.length - 1}{' '}
                  {participants.length > 2 ? 'participants are waiting.' : 'participant is waiting.'}
                  {isModeratorJoined && !isUserModerator && (
                     <span style={{ color: theme.palette.secondary.main }}> Moderator already joined.</span>
                  )}
               </Typography>
            )}
            <div className={classes.fill} />
         </div>
      </ConferenceNotOpenLayout>
   );
}
