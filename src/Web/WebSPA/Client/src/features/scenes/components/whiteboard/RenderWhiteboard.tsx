import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import Whiteboard from 'src/features/whiteboard/components/Whiteboard';
import { selectWhiteboard } from 'src/features/whiteboard/selectors';
import {
   CanvasPushAction,
   WhiteboardLiveActionDto,
   WhiteboardLiveUpdateDto,
   WhiteboardPushActionDto,
} from 'src/features/whiteboard/types';
import { LiveUpdateHandler } from 'src/features/whiteboard/whiteboard-controller';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { WHITEBOARD_CAN_CREATE } from 'src/permissions';
import { RootState } from 'src/store';
import { showMessage } from 'src/store/notifier/actions';
import useSignalRHub from 'src/store/signal/useSignalRHub';
import { formatErrorMessage } from 'src/utils/error-utils';
import { RenderSceneProps, WhiteboardScene } from '../../types';
import AutoSceneLayout from '../AutoSceneLayout';

export default function RenderWhiteboard({ scene, dimensions }: RenderSceneProps<WhiteboardScene>) {
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

   const readOnly = !whiteboard.anyoneCanEdit && !canCreateWhiteboard;

   const handlePushAction = async (action: CanvasPushAction) => {
      if (signalr) {
         const dto: WhiteboardPushActionDto = { action, whiteboardId: scene.id };
         const result = await signalr.invoke<SuccessOrError<{ version: number }>>('WhiteboardPushAction', dto);
         if (result.success) {
            return result.response.version;
         } else {
            dispatch(showMessage({ type: 'error', message: formatErrorMessage(result.error) }));
            throw new Error(result.error.message);
         }
      }
      throw new Error('SignalR not available');
   };

   const handleUndo = () => {
      dispatch(coreHub.whiteboardUndo(scene.id));
   };

   const handleRedo = () => {
      dispatch(coreHub.whiteboardRedo(scene.id));
   };

   const myState = whiteboard.participantStates[myId];

   return (
      <AutoSceneLayout {...dimensions}>
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
      </AutoSceneLayout>
   );
}
