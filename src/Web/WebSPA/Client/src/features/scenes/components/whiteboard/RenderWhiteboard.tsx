import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import Whiteboard from 'src/features/whiteboard/components/Whiteboard';
import { selectWhiteboard } from 'src/features/whiteboard/selectors';
import { CanvasPushAction } from 'src/features/whiteboard/types';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import { RenderSceneProps, WhiteboardScene } from '../../types';

export default function RenderWhiteboard({ scene }: RenderSceneProps<WhiteboardScene>) {
   const whiteboard = useSelector((state: RootState) => selectWhiteboard(state, scene.id));
   const dispatch = useDispatch();
   const myId = useMyParticipantId();

   if (!whiteboard) return null;

   const handlePushAction = (action: CanvasPushAction) => {
      dispatch(coreHub.whiteboardPushAction({ whiteboardId: scene.id, action }));
   };

   const handleUndo = () => {
      dispatch(coreHub.whiteboardUndo(scene.id));
   };

   const handleRedo = () => {
      dispatch(coreHub.whiteboardRedo(scene.id));
   };

   const myState = whiteboard.participantStates[myId];

   return (
      <Whiteboard
         canvas={whiteboard.canvas}
         onPushAction={handlePushAction}
         canUndo={Boolean(myState?.canUndo)}
         canRedo={Boolean(myState?.canRedo)}
         onUndo={handleUndo}
         onRedo={handleRedo}
      />
   );
}
