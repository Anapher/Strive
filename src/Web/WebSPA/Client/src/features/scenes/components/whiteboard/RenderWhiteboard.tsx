import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import Whiteboard from 'src/features/whiteboard/components/Whiteboard';
import { selectWhiteboard } from 'src/features/whiteboard/selectors';
import { CanvasPushAction, WhiteboardLiveActionDto, WhiteboardLiveUpdateDto } from 'src/features/whiteboard/types';
import { LiveUpdateHandler } from 'src/features/whiteboard/whiteboard-controller';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { WHITEBOARD_CAN_CREATE } from 'src/permissions';
import { RootState } from 'src/store';
import useSignalRHub from 'src/store/signal/useSignalRHub';
import { RenderSceneProps, WhiteboardScene } from '../../types';

export default function RenderWhiteboard({ scene }: RenderSceneProps<WhiteboardScene>) {
   const whiteboard = useSelector((state: RootState) => selectWhiteboard(state, scene.id));
   const dispatch = useDispatch();
   const myId = useMyParticipantId();
   const paricipants = useSelector(selectParticipantsOfCurrentRoom);
   const signalr = useSignalRHub();
   const [liveUpdater, setLiveUpdater] = useState<LiveUpdateHandler | undefined>(undefined);
   const canCreateWhiteboard = usePermission(WHITEBOARD_CAN_CREATE);

   useEffect(() => {
      if (!signalr) {
         setLiveUpdater(undefined);
      } else {
         const state: { disposed: boolean; subscriptions: any[] } = {
            disposed: false,
            subscriptions: [],
         };

         setLiveUpdater({
            submit: (action) => {
               const payload: WhiteboardLiveActionDto = { whiteboardId: scene.id, action };
               signalr.send('WhiteboardLiveAction', payload);
            },
            on: (method) => {
               if (state.disposed) return;

               const handler = (arg: WhiteboardLiveUpdateDto) => {
                  if (arg.participantId === myId) return;
                  method(arg);
               };
               signalr.on('OnWhiteboardLiveUpdate', handler);
               state.subscriptions.push(handler);
            },
         });

         return () => {
            state.disposed = true;
            state.subscriptions.forEach((sub) => signalr.off('OnWhiteboardLiveUpdate', sub));
         };
      }
   }, [signalr]);

   if (!whiteboard) return null;

   const readOnly = !whiteboard.everyoneCanEdit && !canCreateWhiteboard;

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
         participants={paricipants}
         liveUpdateHandler={liveUpdater}
         readOnly={readOnly}
      />
   );
}
