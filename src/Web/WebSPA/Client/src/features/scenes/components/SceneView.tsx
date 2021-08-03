import { makeStyles } from '@material-ui/core';
import { AnimateSharedLayout } from 'framer-motion';
import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import presenters from '../scene-presenter-registry';
import { selectSceneStack } from '../selectors';
import { Scene } from '../types';
import SceneSelector from './SceneSelector';

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
});

const getSceneAutoHideControls: (scene: Scene, participantId: string) => boolean | undefined = (
   scene,
   participantId,
) => {
   const presenter = presenters.find((x) => x.type === scene.type);
   if (!presenter?.getAutoHideMediaControls) return undefined;

   return presenter.getAutoHideMediaControls(scene, participantId);
};

type Props = {
   setAutoHideControls: (value: boolean) => void;
};

export default function SceneView({ setAutoHideControls }: Props) {
   const classes = useStyles();
   const [contentRef, dimensions] = useThrottledResizeObserver(100);
   const sceneStack = useSelector(selectSceneStack);

   const participantId = useMyParticipantId();

   useEffect(() => {
      const autoHide =
         sceneStack?.reduceRight<boolean | undefined>(
            (previous, current) =>
               previous !== undefined ? previous : getSceneAutoHideControls(current, participantId),
            undefined,
         ) ?? false;

      setAutoHideControls(autoHide);
   }, [sceneStack, participantId, setAutoHideControls]);

   return (
      <div className={classes.root} ref={contentRef} id="scene-view">
         <AnimateSharedLayout>
            {dimensions && sceneStack ? (
               <SceneSelector className={classes.currentScene} dimensions={dimensions} sceneStack={sceneStack} />
            ) : null}
         </AnimateSharedLayout>
      </div>
   );
}
