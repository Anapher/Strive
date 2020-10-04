import { Link, makeStyles, Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import ParticipantsList from 'src/features/conference/components/ParticipantsList';
import Media from 'src/features/media/components/Media';
import { RootState } from 'src/store';
import { close, joinConference } from 'src/store/conference-signal/actions';
import { IRestError } from 'src/utils/error-result';
import to from 'src/utils/to';

const useStyles = makeStyles({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
   conferenceMain: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row',
   },
   flex: {
      flex: 1,
   },
   chat: {
      flex: 1,
      maxWidth: 360,
      padding: 8,
   },
   participants: {
      flex: 1,
      maxWidth: 180,
   },
   errorRoot: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
   },
   errorContainer: {
      maxWidth: 992,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

type RouteParams = {
   id: string;
};

type Props = RouteComponentProps<RouteParams>;

function ConferenceRoute({
   match: {
      params: { id },
   },
}: Props) {
   const error = useSelector((state: RootState) => state.conference.connectionError);
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(joinConference(id));
      return () => {
         dispatch(close());
      };
   }, [id, joinConference, close]);

   const classes = useStyles();

   if (error) {
      return (
         <div className={classes.errorRoot}>
            <div className={classes.errorContainer}>
               <Typography color="error" variant="h4" align="center" gutterBottom>
                  {error.message}
               </Typography>
               <Link {...to('/')}>Back to start</Link>
            </div>
         </div>
      );
   }

   return (
      <div className={classes.root}>
         <ConferenceAppBar />
         <div className={classes.conferenceMain}>
            <div className={classes.participants}>
               <ParticipantsList />
            </div>
            <div className={classes.flex}>
               <Media />
            </div>
            <div className={classes.chat}>
               <ChatBar />
            </div>
         </div>
      </div>
   );
}

export default ConferenceRoute;
