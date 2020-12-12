import { makeStyles } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import React, { useMemo, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import MediaControls from 'src/features/media/components/MediaControls';
import ParticipantsGrid from './ParticipantsGrid';
import ScreenShare from './ScreenShare';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import { selectCurrentScene } from '../selectors';
import { ViewableScene } from '../types';
import _ from 'lodash';

const AUTO_HIDE_CONTROLS_DELAY_MS = 8000;

const useStyles = makeStyles({
   root: {
      width: '100%',
      height: '100%',
      position: 'relative',
   },
   currentScene: {
      position: 'absolute',
      left: 0,
      top: 0,
      bottom: 0,
      right: 0,
   },
   mediaControls: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
   },
});

type Props = {
   setShowWebcamUnderChat: (show: boolean) => void;
};

export default function SceneView({ setShowWebcamUnderChat }: Props) {
   const classes = useStyles();
   const [contentRef, dimensions] = useThrottledResizeObserver(100);

   let fixedDimensions: Size | undefined;
   if (dimensions && dimensions.width !== undefined && dimensions.height !== undefined)
      fixedDimensions = { width: dimensions.width, height: dimensions.height };

   const currentScene = useSelector(selectCurrentScene);

   const [showControls, setShowControls] = useState(true);
   const autoHideControls = useRef<boolean>(false);

   const delayHideControls = useMemo(
      () =>
         _.debounce(() => {
            if (autoHideControls.current) {
               setShowControls(false);
            }
         }, AUTO_HIDE_CONTROLS_DELAY_MS),
      [setShowControls, autoHideControls],
   );

   const handleSetAutoHideControls = (autoHide: boolean) => {
      autoHideControls.current = autoHide;

      if (autoHide) {
         delayHideControls();
      } else {
         setShowControls(true);
      }
   };

   const handleMouseMove = () => {
      if (!autoHideControls.current) return;

      setShowControls(true);
      delayHideControls();
   };

   return (
      <div className={classes.root} ref={contentRef} onMouseMove={handleMouseMove}>
         <AnimateSharedLayout>
            <SceneSelector
               className={classes.currentScene}
               dimensions={fixedDimensions}
               scene={currentScene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
               setAutoHideControls={handleSetAutoHideControls}
            />
         </AnimateSharedLayout>
         <MediaControls className={classes.mediaControls} show={showControls} />
      </div>
   );
}

type SceneSelectorProps = {
   scene: ViewableScene;
   className?: string;
   dimensions?: Size;
   setShowWebcamUnderChat: (show: boolean) => void;
   setAutoHideControls: (autoHide: boolean) => void;
};

function SceneSelector({
   scene,
   className,
   dimensions,
   setShowWebcamUnderChat,
   setAutoHideControls,
}: SceneSelectorProps) {
   if (!dimensions) return <div />;

   switch (scene.type) {
      case 'grid':
         return (
            <ParticipantsGrid
               className={className}
               dimensions={dimensions}
               options={scene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
               setAutoHideControls={setAutoHideControls}
            />
         );
      case 'screenshare':
         return (
            <ScreenShare
               className={className}
               dimensions={dimensions}
               options={scene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
               setAutoHideControls={setAutoHideControls}
            />
         );
   }
}
