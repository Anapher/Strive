import { makeStyles } from '@material-ui/core';
import React, { useRef, useState } from 'react';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import PinnableSidebar from './PinnableSidebar';
import SceneView from '../../scenes/components/SceneView';
import ParticipantMicManager from 'src/features/media/components/ParticipantMicManager';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
   conferenceMain: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row',
      position: 'relative',
      minHeight: 0,
   },
   scene: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
   },
   chat: {
      flex: 1,
      maxWidth: 360,
   },
}));

export default function ClassConference() {
   const classes = useStyles();
   const [roomsPinned, setRoomsPinned] = useState(true);

   return (
      <ParticipantMicManager>
         <div className={classes.root}>
            <ConferenceAppBar />
            <div className={classes.conferenceMain}>
               <PinnableSidebar pinned={roomsPinned} onTogglePinned={() => setRoomsPinned((x) => !x)} />
               <div className={classes.scene}>
                  <SceneView />
               </div>
               <div className={classes.chat}>
                  <ChatBar />
               </div>
            </div>
         </div>
      </ParticipantMicManager>
   );
}
