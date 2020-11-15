import React, { useEffect, useRef } from 'react';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';
import { ScreenShareScene } from '../types';
import PresentationScene from './PresentationScene';

type Props = {
   className?: string;
   dimensions: Size;
   options: ScreenShareScene;
};

export default function ScreenShare({ className, dimensions, options: { participantId } }: Props) {
   const videoSize: Size = { width: 1920, height: 1080 };

   const videoRef = useRef<HTMLVideoElement | null>(null);
   const consumer = useConsumer(participantId, 'screen');

   useEffect(() => {
      if (consumer && videoRef.current) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         videoRef.current.srcObject = stream;
         videoRef.current.play();
      }
   }, [consumer, videoRef.current]);

   return (
      <PresentationScene
         className={className}
         dimensions={dimensions}
         contentRatio={videoSize}
         maxContentWidth={videoSize.width}
         render={(size) => <video ref={videoRef} style={{ ...size }} />}
      />
   );
}
