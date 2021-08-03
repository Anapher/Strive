import { makeStyles } from '@material-ui/core';
import React, { useMemo, useRef, useState } from 'react';
import AnnouncementOverlay from 'src/features/chat/components/AnnouncementOverlay';
import MediaControlsContext, { MediaControlsContextType } from 'src/features/media/media-controls-context';
import RoomsList from 'src/features/rooms/components/RoomsList';
import SceneView from 'src/features/scenes/components/SceneView';
import ConferenceLayoutContext, { ConferenceLayoutContextType } from '../../conference-layout-context';
import useAutoHideControls from '../../useAutoHideControls';
import MobileChatTab from './MobileChatTab';
import MobileLayoutActions from './MobileLayoutActions';
import MobileMediaControls from './MobileMediaControls';

const AUTO_HIDE_CONTROLS_DELAY_MS = 8000;

const useStyles = makeStyles({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      position: 'relative',
      minHeight: 0,
   },
   actionsOverlay: {
      position: 'absolute',
      bottom: 0,
      left: 0,
      right: 0,
   },
   scene: {
      flex: 1,
      minHeight: 0,
   },
   mediaControls: {
      position: 'absolute',
      bottom: 56,
      left: 0,
      right: 0,
   },
});

const emptyLayoutContext: ConferenceLayoutContextType = {
   chatWidth: 0,
};

export default function MobileLayout() {
   const classes = useStyles();
   const [selectedTab, setSelectedTab] = useState(0);
   const overlayActions = selectedTab === 0;

   const mediaLeftActionsRef = useRef<HTMLDivElement>(null);

   const mediaControlsContextValue = useMemo<MediaControlsContextType>(
      () => ({
         leftControlsContainer: mediaLeftActionsRef.current,
      }),
      [mediaLeftActionsRef.current],
   );

   const { handleMouseMove, setAutoHide, showControls } = useAutoHideControls(AUTO_HIDE_CONTROLS_DELAY_MS);

   return (
      <ConferenceLayoutContext.Provider value={emptyLayoutContext}>
         <MediaControlsContext.Provider value={mediaControlsContextValue}>
            <div className={classes.root}>
               <AnnouncementOverlay />

               {selectedTab === 0 && (
                  <div className={classes.scene} onMouseMove={handleMouseMove}>
                     <SceneView setAutoHideControls={setAutoHide} />
                  </div>
               )}
               {selectedTab === 1 && <MobileChatTab />}
               {selectedTab === 2 && <RoomsList />}

               <MobileMediaControls
                  className={classes.mediaControls}
                  show={showControls && selectedTab === 0}
                  leftActionsRef={mediaLeftActionsRef}
               />

               <div className={overlayActions ? classes.actionsOverlay : undefined}>
                  <MobileLayoutActions selectedTab={selectedTab} setSelectedTab={setSelectedTab} />
               </div>
            </div>
         </MediaControlsContext.Provider>
      </ConferenceLayoutContext.Provider>
   );
}
