import React from 'react';
import { useSelector } from 'react-redux';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import { selectParticipant } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';
import { selectSceneOptions } from '../../selectors';
import { RenderSceneProps, ScreenShareScene } from '../../types';
import PresentationSceneMaxSize from '../PresentationSceneMaxSize';

export default function ScreenShare({ className, dimensions, scene }: RenderSceneProps<ScreenShareScene>) {
   const { participantId } = scene;

   const videoSize: Size = { width: 1920, height: 1080 };
   const consumer = useConsumer(participantId, 'screen');

   const participant = useSelector((state: RootState) => selectParticipant(state, participantId));
   const sceneOptions = useSelector(selectSceneOptions);

   return (
      <PresentationSceneMaxSize
         className={className}
         maxOverlayFactor={sceneOptions?.overlayScene ? 0.75 : 0}
         fixedParticipants={participant && [participant]}
         dimensions={dimensions}
         contentRatio={videoSize}
         maxContentWidth={videoSize.width}
         render={(size, style) => (
            <div style={{ position: 'relative', ...size, ...style }}>
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
