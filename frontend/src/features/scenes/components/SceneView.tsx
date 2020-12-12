import { makeStyles } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import React from 'react';
import { useSelector } from 'react-redux';
import MediaControls from 'src/features/media/components/MediaControls';
import ParticipantsGrid from './ParticipantsGrid';
import ScreenShare from './ScreenShare';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { Size } from 'src/types';
import { selectCurrentScene } from '../selectors';
import { ViewableScene } from '../types';

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
      left: 16,
      right: 16,
      bottom: 16,
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

   return (
      <div className={classes.root} ref={contentRef}>
         <AnimateSharedLayout>
            <SceneSelector
               className={classes.currentScene}
               dimensions={fixedDimensions}
               scene={currentScene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
            />
         </AnimateSharedLayout>
         <MediaControls className={classes.mediaControls} />
      </div>
   );
}

type SceneSelectorProps = {
   scene: ViewableScene;
   className?: string;
   dimensions?: Size;
   setShowWebcamUnderChat: (show: boolean) => void;
};

function SceneSelector({ scene, className, dimensions, setShowWebcamUnderChat }: SceneSelectorProps) {
   if (!dimensions) return <div />;

   switch (scene.type) {
      case 'grid':
         return (
            <ParticipantsGrid
               className={className}
               dimensions={dimensions}
               options={scene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
            />
         );
      case 'screenshare':
         return (
            <ScreenShare
               className={className}
               dimensions={dimensions}
               options={scene}
               setShowWebcamUnderChat={setShowWebcamUnderChat}
            />
         );
   }
}
