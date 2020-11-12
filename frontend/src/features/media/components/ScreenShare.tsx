import React from 'react';
import { ScreenShareScene } from 'src/features/conference/types';
import { Size } from 'src/types';
import PresentationScene from './PresentationScene';

type Props = {
   className?: string;
   dimensions: Size;
   options: ScreenShareScene;
};

export default function ScreenShare({ className, dimensions, options }: Props) {
   const videoSize: Size = { width: 1920, height: 1080 };
   return (
      <PresentationScene
         className={className}
         dimensions={dimensions}
         contentRatio={videoSize}
         maxContentWidth={videoSize.width}
         render={(size) => <div style={{ ...size, backgroundColor: 'blue' }} />}
      />
   );
}
