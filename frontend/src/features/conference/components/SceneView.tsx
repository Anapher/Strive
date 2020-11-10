import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import MediaControls from 'src/features/media/components/MediaControls';
import ParticipantsGrid from 'src/features/media/components/ParticipantsGrid';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';

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

   const fixedDimensions = dimensions &&
      dimensions.width &&
      dimensions.height && { width: dimensions.width - 32, height: dimensions.height - 32 };

   return (
      <div className={classes.root} ref={contentRef}>
         {dimensions && <ParticipantsGrid className={classes.currentScene} dimensions={fixedDimensions as any} />}
         <MediaControls className={classes.mediaControls} />
      </div>
   );
}
