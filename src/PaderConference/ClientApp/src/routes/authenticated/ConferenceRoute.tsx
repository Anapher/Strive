import { Link, makeStyles, Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import ClassConference from 'src/features/conference/components/ClassConference';
import ConferenceConnecting from 'src/features/conference/components/ConferenceConnecting';
import ConferenceNotOpen from 'src/features/conference/components/ConferenceNotOpen';
import { RootState } from 'src/store';
import { close, joinConference } from 'src/store/conference-signal/actions';
import to from 'src/utils/to';

const useStyles = makeStyles({
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
   const conferenceState = useSelector((state: RootState) => state.conference.conferenceState);
   const { isConnected, isReconnecting } = useSelector((state: RootState) => state.signalr);

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

   if (!conferenceState || !isConnected) {
      return <ConferenceConnecting isReconnecting={isReconnecting} />;
   }

   if (!conferenceState.isOpen) {
      return <ConferenceNotOpen conferenceInfo={conferenceState} />;
   }

   switch (conferenceState.conferenceType) {
      default:
         return <ClassConference />;
   }
}

export default ConferenceRoute;
