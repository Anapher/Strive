import { Grid, makeStyles } from '@material-ui/core';
import { motion } from 'framer-motion';
import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import AnnouncementOverlay from 'src/features/chat/components/AnnouncementOverlay';
import { selectShowChat } from 'src/features/chat/selectors';
import MediaControlsContext, { MediaControlsContextType } from 'src/features/media/media-controls-context';
import CurrentPollsBar from 'src/features/poll/components/CurrentPollsBar';
import { expandToBox } from 'src/features/scenes/calculations';
import { ACTIVE_CHIPS_LAYOUT_HEIGHT } from 'src/features/scenes/components/ActiveChipsLayout';
import SceneView from 'src/features/scenes/components/SceneView';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import ConferenceLayoutContext, { ConferenceLayoutContextType } from '../../conference-layout-context';
import useAutoHideControls from '../../useAutoHideControls';
import ConferenceAppBar from '../ConferenceAppBar';
import ConferenceSidebar from '../ConferenceSidebar';
import PermissionDialog from '../PermissionDialog';
import DesktopChatBar from './DesktopChatBar';
import DesktopMediaControls from './DesktopMediaControls';

const CHAT_MIN_WIDTH = 304;
const CHAT_MAX_WIDTH = 416;
const CHAT_DEFAULT_WIDTH = 320;

const AUTO_HIDE_CONTROLS_DELAY_MS = 8000;

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
   sceneLayout: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row',
      minHeight: 0,
   },
   scene: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      minWidth: 0,
      position: 'relative',
   },
   sceneContainer: {
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
   mediaControls: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
   },
   mediaControlsBackground: {
      backgroundImage: 'linear-gradient(to bottom, rgba(6, 6, 7, 0), rgba(6, 6, 7, 0.4), rgba(6, 6, 7, 0.7))',
      height: 80,
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      zIndex: -1000,
   },
}));

function computeChatWidth(dimensions: Size) {
   const computedSize = expandToBox(defaultContentRatio, dimensions);

   let newChatWidth = dimensions.width - computedSize.width;
   if (newChatWidth < CHAT_MIN_WIDTH) newChatWidth = CHAT_MIN_WIDTH;
   if (newChatWidth > CHAT_MAX_WIDTH) newChatWidth = CHAT_MAX_WIDTH;

   return newChatWidth;
}

export default function DesktopLayout() {
   const classes = useStyles();

   const showChat = useSelector(selectShowChat);

   const [contentRef, dimensions] = useThrottledResizeObserver(100);
   const [chatWidth, setChatWidth] = useState(CHAT_DEFAULT_WIDTH);

   useEffect(() => {
      if (dimensions) {
         const newWidth = computeChatWidth({
            width: dimensions.width,
            height: dimensions.height - ACTIVE_CHIPS_LAYOUT_HEIGHT /** for chips and the arrow back */,
         });

         setChatWidth(newWidth);
      }
   }, [dimensions?.width, dimensions?.height]);

   const chatContainer = useRef<HTMLDivElement>(null);
   const sceneBarContainer = useRef<HTMLDivElement>(null);

   const context = useMemo<ConferenceLayoutContextType>(
      () => ({
         chatContainer: chatContainer.current,
         chatWidth,
         sceneBarContainer: sceneBarContainer.current,
         sceneBarWidth: dimensions?.width,
      }),
      [chatContainer.current, chatWidth, dimensions?.width],
   );

   const mediaLeftActionsRef = useRef<HTMLDivElement>(null);

   const { handleMouseMove, setAutoHide, showControls } = useAutoHideControls(AUTO_HIDE_CONTROLS_DELAY_MS);

   const mediaControlsContextValue = useMemo<MediaControlsContextType>(
      () => ({
         leftControlsContainer: mediaLeftActionsRef.current,
      }),
      [mediaLeftActionsRef.current],
   );

   return (
      <ConferenceLayoutContext.Provider value={context}>
         <div className={classes.root}>
            <AnnouncementOverlay />
            <ConferenceAppBar chatWidth={chatWidth} />
            <div className={classes.conferenceMain}>
               <motion.div
                  className={classes.mediaControlsBackground}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: showControls ? 1 : 0 }}
               />
               <ConferenceSidebar />
               <div className={classes.sceneContainer}>
                  <div ref={sceneBarContainer} />
                  <div className={classes.sceneLayout} ref={contentRef}>
                     <MediaControlsContext.Provider value={mediaControlsContextValue}>
                        <div className={classes.scene} onMouseMove={handleMouseMove}>
                           <SceneView setAutoHideControls={setAutoHide} />
                           <DesktopMediaControls
                              className={classes.mediaControls}
                              show={showControls}
                              leftActionsRef={mediaLeftActionsRef}
                           />
                        </div>
                     </MediaControlsContext.Provider>
                     {showChat && (
                        <div className={classes.chat} style={{ width: chatWidth }}>
                           <Grid ref={chatContainer} />
                           <CurrentPollsBar />
                           <DesktopChatBar />
                        </div>
                     )}
                  </div>
               </div>
            </div>
            <PermissionDialog />
         </div>
      </ConferenceLayoutContext.Provider>
   );
}
