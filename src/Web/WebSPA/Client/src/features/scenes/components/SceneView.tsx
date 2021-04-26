import { makeStyles } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import _ from 'lodash';
import React, { useMemo, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import MediaControls from 'src/features/media/components/MediaControls';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import { selectSceneStack } from '../selectors';
import SceneSelector from './SceneSelector';

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

   const sceneStack = useSelector(selectSceneStack);

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
            {fixedDimensions?.width !== undefined && fixedDimensions?.height !== undefined && sceneStack ? (
               <SceneSelector
                  className={classes.currentScene}
                  dimensions={fixedDimensions}
                  sceneStack={sceneStack}
                  setShowWebcamUnderChat={setShowWebcamUnderChat}
                  setAutoHideControls={handleSetAutoHideControls}
               />
            ) : null}
         </AnimateSharedLayout>
         <MediaControls className={classes.mediaControls} show={showControls} />
      </div>
   );
}
