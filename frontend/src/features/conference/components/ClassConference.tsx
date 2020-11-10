import { makeStyles } from '@material-ui/core';
import React, { useRef, useState } from 'react';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import PinnableSidebar from './PinnableSidebar';
import SceneView from './SceneView';

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
   },
   scene: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
   },
   chat: {
      flex: 1,
      maxWidth: 360,
      padding: theme.spacing(1, 1, 1, 0),
   },
}));

export default function ClassConference() {
   const classes = useStyles();
   const [roomsPinned, setRoomsPinned] = useState(true);
   const hamburgerRef = useRef<HTMLButtonElement>(null);

   const handleGetHamburger = () => hamburgerRef.current;

   return (
      <div className={classes.root}>
         <ConferenceAppBar hamburgerRef={hamburgerRef} />
         <div className={classes.conferenceMain}>
            <PinnableSidebar
               pinned={roomsPinned}
               onTogglePinned={() => setRoomsPinned((x) => !x)}
               getHamburger={handleGetHamburger}
            />
            <div className={classes.scene}>
               <SceneView />
            </div>
            <div className={classes.chat}>
               <ChatBar />
            </div>
         </div>
      </div>
   );
}
