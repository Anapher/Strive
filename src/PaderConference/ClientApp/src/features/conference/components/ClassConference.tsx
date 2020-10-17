import { makeStyles } from '@material-ui/core';
import React from 'react';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import Media from 'src/features/media/components/Media';
import RoomsList from 'src/features/rooms/components/RoomsList';

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
});

export default function ClassConference() {
   const classes = useStyles();

   return (
      <div className={classes.root}>
         <ConferenceAppBar />
         <div className={classes.conferenceMain}>
            <div className={classes.participants}>
               <RoomsList />
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
