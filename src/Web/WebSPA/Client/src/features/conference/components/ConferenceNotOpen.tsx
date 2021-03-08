import {
   Box,
   Button,
   Checkbox,
   Container,
   FormControlLabel,
   IconButton,
   makeStyles,
   Typography,
   useTheme,
} from '@material-ui/core';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import _ from 'lodash';
import { DateTime } from 'luxon';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { openSettings } from 'src/features/settings/reducer';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/permissions';
import { SynchronizedConferenceInfo } from 'src/store/signal/synchronization/synchronized-object-ids';
import to from 'src/utils/to';
import { selectParticipants } from '../selectors';

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
         <div>
            <Typography variant="h4">
               The conference is scheduled for {date.toLocaleString(DateTime.DATETIME_FULL)}
            </Typography>
         </div>
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
   const myId = useMyParticipantId();

   const isUserModerator = conferenceInfo.moderators.includes(myId);
   const isModeratorJoined = _.some(participants, (x) => conferenceInfo.moderators.includes(x.id));

   const theme = useTheme();

   const dispatch = useDispatch();
   const handleOpenConference = () => dispatch(coreHub.openConference());
   const canOpen = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);

   const handleOpenSettings = () => dispatch(openSettings());

   return (
      <div className={classes.root}>
         <Box display="flex" flexDirection="row" position="absolute" right={32} top={32}>
            <Button onClick={handleOpenSettings}>Settings</Button>
         </Box>
         <Box display="flex" flexDirection="row" position="absolute" left={32} top={32}>
            <IconButton {...to('/')}>
               <ArrowBackIcon />
            </IconButton>
         </Box>
         <Box display="flex" flexDirection="row" position="absolute" left={32} bottom={32}>
            <FormControlLabel control={<Checkbox checked={true} />} label="Play a sound when the conference opens" />
         </Box>
         <Container maxWidth="md" className={classes.container}>
            <div className={classes.topContent}>
               {isUserModerator && <Typography gutterBottom>You are a moderator of this conference.</Typography>}
            </div>
            <div>
               {canOpen ? (
                  <Button variant="contained" color="primary" onClick={handleOpenConference}>
                     Open conference
                  </Button>
               ) : (
                  <ConferenceStatus conferenceInfo={conferenceInfo} isModeratorJoined={isModeratorJoined} />
               )}
            </div>
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
               {!isUserModerator && (
                  <Box mt={6}>
                     <Typography color="textSecondary" align="center">
                        {"You don't need to refresh this page"}
                     </Typography>
                  </Box>
               )}
            </div>
         </Container>
      </div>
   );
}
