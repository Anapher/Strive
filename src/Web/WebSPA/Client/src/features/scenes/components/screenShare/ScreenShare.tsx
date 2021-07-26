import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { useContext } from 'react';
import { useSelector } from 'react-redux';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import { selectParticipant } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';
import { expandToBox } from '../../calculations';
import LayoutChildSizeContext from '../../layout-child-size-context';
import { RenderSceneProps, ScreenShareScene } from '../../types';
import AutoSceneLayout from '../AutoSceneLayout';

const defaultVideoSize: Size = { width: 1920, height: 1080 };

function getVideoSize(consumer: Consumer | null | undefined): Size {
   const settings = consumer?.track.getSettings();
   if (typeof settings?.width === 'number' && typeof settings?.height === 'number') {
      return { width: settings.width, height: settings.height };
   }

   return defaultVideoSize;
}

export default function ScreenShare({ className, dimensions, scene }: RenderSceneProps<ScreenShareScene>) {
   const { participantId } = scene;
   const consumer = useConsumer(participantId, 'screen');
   const participant = useSelector((state: RootState) => selectParticipant(state, participantId));

   return (
      <AutoSceneLayout {...dimensions} className={className} participant={participant}>
         <RenderSceen consumer={consumer} />
      </AutoSceneLayout>
   );
}

type RenderSceenProps = {
   consumer: Consumer | undefined | null;
};
function RenderSceen({ consumer }: RenderSceenProps) {
   const size = useContext(LayoutChildSizeContext);
   const videoSize = getVideoSize(consumer);
   const renderSize = expandToBox(videoSize, size);

   return (
      <div style={{ position: 'relative', ...renderSize }}>
         <RenderConsumerVideo
            id="video-screenshare"
            consumer={consumer}
            height={renderSize.height}
            width={renderSize.width}
            diagnosticsLocation="top-right"
            videoContain
         />
      </div>
   );
}
