import { makeStyles } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import _ from 'lodash';
import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import MediaControls from 'src/features/media/components/MediaControls';
import MediaControlsContext, { MediaControlsContextType } from 'src/features/media/media-controls-context';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import presenters from '../scene-presenter-registry';
import { selectSceneStack } from '../selectors';
import { Scene } from '../types';
import SceneSelector from './SceneSelector';

const AUTO_HIDE_CONTROLS_DELAY_MS = 8000;

const useStyles = makeStyles({
   root: {
      width: '100%',
      height: '100%',
      position: 'relative',
      overflow: 'hidden',
   },
   currentScene: {
      position: 'absolute',
      left: 0,
      top: 0,
      bottom: 0,
      right: 0,
      overflow: 'hidden',
   },
   mediaControls: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
   },
});

const getSceneAutoHideControls: (scene: Scene, participantId: string) => boolean | undefined = (
   scene,
   participantId,
) => {
   const presenter = presenters.find((x) => x.type === scene.type);
   if (!presenter?.getAutoHideMediaControls) return undefined;

   return presenter.getAutoHideMediaControls(scene, participantId);
};

export default function SceneView() {
   const classes = useStyles();
   const [contentRef, dimensions] = useThrottledResizeObserver(100);

   let fixedDimensions: Size | undefined;
   if (dimensions && dimensions.width !== undefined && dimensions.height !== undefined)
      fixedDimensions = { width: dimensions.width, height: dimensions.height };

   const sceneStack = useSelector(selectSceneStack);

   const [showControls, setShowControls] = useState(true);
   const autoHideControls = useRef<boolean>(false);
   const mediaLeftActionsRef = useRef<HTMLDivElement>(null);
   const participantId = useMyParticipantId();

   const delayHideControlsFactory = () =>
      _.debounce(() => {
         if (autoHideControls.current) {
            setShowControls(false);
         }
      }, AUTO_HIDE_CONTROLS_DELAY_MS);

   const delayHideControls = useRef<_.DebouncedFunc<() => void>>(delayHideControlsFactory());

   useEffect(() => {
      delayHideControls.current.cancel();

      const fn = delayHideControlsFactory();
      delayHideControls.current = fn;

      return () => fn.cancel();
   }, [setShowControls, autoHideControls]);

   const handleSetAutoHideControls = (autoHide: boolean) => {
      autoHideControls.current = autoHide;

      if (autoHide) {
         delayHideControls.current();
      } else {
         setShowControls(true);
      }
   };

   useEffect(() => {
      const autoHide =
         sceneStack?.reduceRight<boolean | undefined>(
            (previous, current) =>
               previous !== undefined ? previous : getSceneAutoHideControls(current, participantId),
            undefined,
         ) ?? false;

      handleSetAutoHideControls(autoHide);
   }, [sceneStack]);

   const mediaControlsContextValue = useMemo<MediaControlsContextType>(
      () => ({
         leftControlsContainer: mediaLeftActionsRef.current,
      }),
      [mediaLeftActionsRef.current],
   );

   const handleMouseMove = () => {
      if (!autoHideControls.current) return;

      setShowControls(true);
      delayHideControls.current();
   };

   return (
      <MediaControlsContext.Provider value={mediaControlsContextValue}>
         <div className={classes.root} ref={contentRef} onMouseMove={handleMouseMove} id="scene-view">
            <AnimateSharedLayout>
               {fixedDimensions?.width !== undefined && fixedDimensions?.height !== undefined && sceneStack ? (
                  <SceneSelector
                     className={classes.currentScene}
                     dimensions={fixedDimensions}
                     sceneStack={sceneStack}
                  />
               ) : null}
            </AnimateSharedLayout>
            <MediaControls className={classes.mediaControls} show={showControls} leftActionsRef={mediaLeftActionsRef} />
         </div>
      </MediaControlsContext.Provider>
   );
}
