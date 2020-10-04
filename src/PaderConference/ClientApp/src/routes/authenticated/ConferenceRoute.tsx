import { makeStyles } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RouteComponentProps } from 'react-router-dom';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import { RootState } from 'src/store';
import { close, joinConference } from 'src/store/conference-signal/actions';

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
      maxWidth: 300,
      padding: 8,
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
   const connected = useSelector<RootState>((state) => state.signalr.isConnected);
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(joinConference(id));
      return () => {
         dispatch(close());
      };
   }, [id, joinConference, close]);

   const classes = useStyles();

   return (
      <div className={classes.root}>
         <ConferenceAppBar />
         <div className={classes.conferenceMain}>
            <div className={classes.flex}></div>
            <div className={classes.flex}></div>
            <div className={classes.chat}>
               <ChatBar />
            </div>
         </div>
      </div>
   );
}

export default ConferenceRoute;
