import { Grid, makeStyles } from '@material-ui/core';
import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import AnnouncementOverlay from 'src/features/chat/components/AnnouncementOverlay';
import ChatBar from 'src/features/chat/components/ChatBar';
import { selectShowChat } from 'src/features/chat/selectors';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import ParticipantMicManager from 'src/features/media/components/ParticipantMicManager';
import CurrentPollsBar from 'src/features/poll/components/CurrentPollsBar';
import { expandToBox } from 'src/features/scenes/calculations';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import SceneView from '../../scenes/components/SceneView';
import ConferenceLayoutContext, { ConferenceLayoutContextType } from '../conference-layout-context';
import PermissionDialog from './PermissionDialog';
import PinnableSidebar from './PinnableSidebar';

const CHAT_MIN_WIDTH = 304;
const CHAT_MAX_WIDTH = 416;
const CHAT_DEFAULT_WIDTH = 320;

// optimize for 16:9
const defaultContentRatio: Size = { width: 16, height: 9 };

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
      minWidth: 0,
   },
   chat: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
      padding: theme.spacing(1, 1, 1, 0),
   },
}));

export default function ClassConference() {
   const classes = useStyles();
   const [roomsPinned, setRoomsPinned] = useState(true);

   const showChat = useSelector(selectShowChat);

   const [contentRef, dimensions] = useThrottledResizeObserver(100);
   const [chatWidth, setChatWidth] = useState(CHAT_DEFAULT_WIDTH);

   useEffect(() => {
      let fixedDimensions: Size | undefined;
      if (dimensions && dimensions.width !== undefined && dimensions.height !== undefined)
         fixedDimensions = {
            width: dimensions.width,
            height: dimensions.height - 40 /** for chips and the arrow back */,
         };

      if (fixedDimensions) {
         const computedSize = expandToBox(defaultContentRatio, fixedDimensions);

         let newChatWidth = fixedDimensions.width - computedSize.width;
         if (newChatWidth < CHAT_MIN_WIDTH) newChatWidth = CHAT_MIN_WIDTH;
         if (newChatWidth > CHAT_MAX_WIDTH) newChatWidth = CHAT_MAX_WIDTH;

         setChatWidth(newChatWidth);
      }
   }, [dimensions.width, dimensions.height]);

   const chatContainer = useRef<HTMLDivElement>(null);

   const context = useMemo<ConferenceLayoutContextType>(
      () => ({
         chatContainer: chatContainer.current,
         chatWidth,
      }),
      [chatContainer.current, chatWidth],
   );

   return (
      <ParticipantMicManager>
         <ConferenceLayoutContext.Provider value={context}>
            <div className={classes.root}>
               <AnnouncementOverlay />
               <ConferenceAppBar chatWidth={chatWidth} />
               <div className={classes.conferenceMain} ref={contentRef}>
                  <PinnableSidebar pinned={roomsPinned} onTogglePinned={() => setRoomsPinned((x) => !x)} />
                  <div className={classes.scene}>
                     <SceneView />
                  </div>
                  {showChat && (
                     <div className={classes.chat} style={{ width: chatWidth }}>
                        <Grid ref={chatContainer} />
                        <CurrentPollsBar />
                        <ChatBar />
                     </div>
                  )}
               </div>
               <PermissionDialog />
            </div>
         </ConferenceLayoutContext.Provider>
      </ParticipantMicManager>
   );
}
