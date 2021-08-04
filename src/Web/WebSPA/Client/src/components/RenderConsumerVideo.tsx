import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import useResponsiveConsumer from 'src/features/media/useResponsiveConsumer';
import { selectEnableVideoOverlay } from 'src/features/settings/selectors';
import ConsumerDiagnosticInfo from './ConsumerDiagnosticInfo';

const useStyles = makeStyles(() => ({
   video: {
      position: 'absolute',
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: '100%',
      height: '100%',
      objectFit: 'cover',
   },
   consumerInfoBottomRight: {
      position: 'absolute',
      right: 0,
      bottom: 0,
   },
   consumerInfoTopRight: {
      position: 'absolute',
      right: 0,
      top: 0,
   },
}));

type Props = Omit<Omit<React.HTMLProps<HTMLVideoElement>, 'hidden'>, 'style'> & {
   consumer?: Consumer | null;
   className?: string;

   videoContain?: boolean;

   diagnosticsLocation?: 'bottom-right' | 'top-right';
};

export default function RenderConsumerVideo({
   consumer,
   className,
   videoContain,
   diagnosticsLocation = 'bottom-right',
   ...props
}: Props) {
   const classes = useStyles();

   const videoRef = useRef<HTMLVideoElement | null>(null);
   const showDiagnostics = useSelector(selectEnableVideoOverlay);

   useResponsiveConsumer(videoRef, consumer);

   useEffect(() => {
      if (consumer?.track) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         if (videoRef.current) {
            videoRef.current.srcObject = stream;
         }
      }
   }, [consumer]);

   const isActive = consumer?.paused === false;

   return (
      <>
         <video
            ref={videoRef}
            className={clsx(className, classes.video)}
            hidden={!isActive}
            autoPlay
            style={videoContain ? { objectFit: 'contain' } : undefined}
            {...props}
         />
         {consumer && showDiagnostics && (
            <div
               className={clsx({
                  [classes.consumerInfoBottomRight]: diagnosticsLocation === 'bottom-right',
                  [classes.consumerInfoTopRight]: diagnosticsLocation === 'top-right',
               })}
            >
               <ConsumerDiagnosticInfo consumer={consumer} videoElement={videoRef} />
            </div>
         )}
      </>
   );
}
