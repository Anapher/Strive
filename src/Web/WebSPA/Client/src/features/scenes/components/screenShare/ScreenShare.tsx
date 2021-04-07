import React, { useEffect, useState } from 'react';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';
import { RenderSceneProps, ScreenShareScene } from '../../types';
import PresentationScene from '../PresentationScene';

export default function ScreenShare({
   className,
   dimensions,
   scene,
   setShowWebcamUnderChat,
   setAutoHideControls,
}: RenderSceneProps) {
   const { participantId } = scene as ScreenShareScene;

   const videoSize: Size = { width: 1920, height: 1080 };
   const consumer = useConsumer(participantId, 'screen');

   const [showParticipantOverlay, setShowParticipantOverlay] = useState(false);

   useEffect(() => {
      setAutoHideControls(true);
   }, [setAutoHideControls]);

   const handleCanShowParticipantsWithoutResize = (canShow: boolean) => {
      setShowParticipantOverlay(canShow);
      setShowWebcamUnderChat(!canShow);
   };

   return (
      <PresentationScene
         className={className}
         showParticipants={showParticipantOverlay}
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
