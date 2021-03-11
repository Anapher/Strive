import { makeStyles } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import AnnouncementOverlay from 'src/features/chat/components/AnnouncementOverlay';
import ChatBar from 'src/features/chat/components/ChatBar';
import ConferenceAppBar from 'src/features/conference/components/ConferenceAppBar';
import DiagnosticsWindow from 'src/features/diagnostics/components/DiagnosticsWindow';
import ParticipantMicManager from 'src/features/media/components/ParticipantMicManager';
import { expandToBox } from 'src/features/scenes/calculations';
import ActiveParticipantsGridSizer from 'src/features/scenes/components/ActiveParticipantsGridSizer';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import SceneView from '../../scenes/components/SceneView';
import PermissionDialog from './PermissionDialog';
import PinnableSidebar from './PinnableSidebar';

const CHAT_MIN_WIDTH = 304;
const CHAT_MAX_WIDTH = 416;
const CHAT_DEFAULT_WIDTH = 320;

// optimize for 16:9
const defaultContentRatio: Size = { width: 16, height: 9 };

const useStyles = makeStyles(() => ({
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
}));

export default function ClassConference() {
   const classes = useStyles();
   const [roomsPinned, setRoomsPinned] = useState(true);
   const [showUnderChat, setShowUnderChat] = useState(false);

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

   return (
      <ParticipantMicManager>
         <div className={classes.root}>
            <AnnouncementOverlay />
            <ConferenceAppBar chatWidth={chatWidth} />
            <div className={classes.conferenceMain} ref={contentRef}>
               <PinnableSidebar pinned={roomsPinned} onTogglePinned={() => setRoomsPinned((x) => !x)} />
               <div className={classes.scene}>
                  <SceneView setShowWebcamUnderChat={(show) => setShowUnderChat(show)} />
               </div>
               <div style={{ width: chatWidth, display: 'flex', flexDirection: 'column', height: '100%' }}>
                  <ActiveParticipantsGridSizer show={showUnderChat} width={chatWidth} />
                  <ChatBar />
               </div>
            </div>
            <PermissionDialog />
            <DiagnosticsWindow />
         </div>
      </ParticipantMicManager>
   );
}
