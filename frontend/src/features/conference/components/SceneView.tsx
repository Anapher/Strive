import { makeStyles, Typography } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import React from 'react';
import MediaControls from 'src/features/media/components/MediaControls';
import ParticipantsGrid from 'src/features/media/components/ParticipantsGrid';
import ScreenShare from 'src/features/media/components/ScreenShare';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import { Scene } from '../types';

const useStyles = makeStyles({
   root: {
      width: '100%',
      height: '100%',
      position: 'relative',
   },
   currentScene: {
      position: 'absolute',
      left: 16,
      top: 16,
      bottom: 16,
      right: 16,
   },
   mediaControls: {
      position: 'absolute',
      left: 16,
      right: 16,
      bottom: 16,
   },
});

export default function SceneView() {
   const classes = useStyles();
   const [contentRef, dimensions] = useThrottledResizeObserver(100);

   let fixedDimensions: Size | undefined;
   if (dimensions && dimensions.width !== undefined && dimensions.height !== undefined)
      fixedDimensions = { width: dimensions.width - 32, height: dimensions.height - 32 };

   const scene: Scene = { type: 'grid' };

   return (
      <div className={classes.root} ref={contentRef}>
         <AnimateSharedLayout>
            <SceneSelector className={classes.currentScene} dimensions={fixedDimensions} scene={scene} />
         </AnimateSharedLayout>
         <MediaControls className={classes.mediaControls} />
      </div>
   );
}

type SceneSelectorProps = {
   scene: Scene;
   className?: string;
   dimensions?: Size;
};

function SceneSelector({ scene, className, dimensions }: SceneSelectorProps) {
   if (!dimensions) return <div />;

   switch (scene.type) {
      case 'grid':
         return <ParticipantsGrid className={className} dimensions={dimensions} options={scene} />;
      case 'screenshare':
         return <ScreenShare className={className} dimensions={dimensions} options={scene} />;
      default:
         break;
   }

   return <div />;
}
