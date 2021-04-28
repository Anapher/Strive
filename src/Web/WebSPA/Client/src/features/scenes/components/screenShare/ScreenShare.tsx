import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import { selectParticipants } from 'src/features/conference/selectors';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';
import { RenderSceneProps, ScreenShareScene } from '../../types';
import PresentationScene from '../PresentationScene';

export default function ScreenShare({
   className,
   dimensions,
   scene,
   next,
   setShowWebcamUnderChat,
}: RenderSceneProps<ScreenShareScene>) {
   const { participantId } = scene;

   const videoSize: Size = { width: 1920, height: 1080 };
   const consumer = useConsumer(participantId, 'screen');

   const [showParticipantOverlay, setShowParticipantOverlay] = useState(false);
   const participants = useSelector(selectParticipants);

   const handleCanShowParticipantsWithoutResize = (canShow: boolean) => {
      setShowParticipantOverlay(canShow);
      setShowWebcamUnderChat(!canShow);
   };

   const overwrite = next({ appliedShowMediaControls: true });
   if (overwrite) return <>{overwrite}</>;

   return (
      <PresentationScene
         className={className}
         showParticipants={showParticipantOverlay}
         fixedParticipants={participants.filter((x) => x.id === participantId)}
         canShowParticipantsWithoutResize={handleCanShowParticipantsWithoutResize}
         dimensions={dimensions}
         contentRatio={videoSize}
         maxContentWidth={videoSize.width}
         render={(size) => (
            <div style={{ position: 'relative', ...size }}>
               <RenderConsumerVideo
                  consumer={consumer}
                  height={size.height}
                  width={size.width}
                  diagnosticsLocation="top-right"
               />
            </div>
         )}
      />
   );
}
