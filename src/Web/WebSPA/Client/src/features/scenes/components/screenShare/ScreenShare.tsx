import React, { useEffect, useRef, useState } from 'react';
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

   const videoRef = useRef<HTMLVideoElement | null>(null);
   const consumer = useConsumer(participantId, 'screen');

   const [showParticipantOverlay, setShowParticipantOverlay] = useState(false);

   useEffect(() => {
      setAutoHideControls(true);
   }, [setAutoHideControls]);

   useEffect(() => {
      if (consumer && videoRef.current) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         videoRef.current.srcObject = stream;
         videoRef.current.play();
      }
   }, [consumer, videoRef.current]);

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
         render={(size) => <video ref={videoRef} style={{ ...size }} />}
      />
   );
}
