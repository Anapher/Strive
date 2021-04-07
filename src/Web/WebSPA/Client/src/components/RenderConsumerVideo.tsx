import { makeStyles } from '@material-ui/core';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { useEffect, useRef } from 'react';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import ConsumerDiagnosticInfo from './ConsumerDiagnosticInfo';
import clsx from 'classnames';
import { useSelector } from 'react-redux';
import { selectEnableVideoOverlay } from 'src/features/settings/selectors';
import { ProducerDevice, ProducerSource } from 'src/store/webrtc/types';
import { layerResolutions as screenResolutions } from 'src/store/webrtc/hooks/useScreen';
import { layerResolutions as webcamResolutions } from 'src/store/webrtc/hooks/useWebcam';
import { parseScalabilityMode } from 'mediasoup-client';
import { ScalabilityMode } from 'mediasoup-client/lib/scalabilityModes';

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

const computePrefferredLayerForResolutions = (
   height: number,
   scalabilityMode: ScalabilityMode,
   resolutions: number[],
) => {
   for (let i = 0; i < Math.min(scalabilityMode.spatialLayers, resolutions.length); i++) {
      const resolution = resolutions[i];

      if (resolution < height) continue;

      return i;
   }

   return scalabilityMode.spatialLayers - 1;
};

const computePrefferredLayerForDevice = (height: number, scalabilityMode: ScalabilityMode, device: ProducerDevice) => {
   switch (device) {
      case 'screen':
         return computePrefferredLayerForResolutions(height, scalabilityMode, screenResolutions);
      case 'webcam':
         return computePrefferredLayerForResolutions(height, scalabilityMode, webcamResolutions);
      default:
         return 0;
   }
};

const getProducerDevice: (source: ProducerSource) => ProducerDevice = (source) => {
   switch (source) {
      case 'mic':
      case 'webcam':
      case 'screen':
         return source;
      case 'loopback-mic':
         return 'mic';
      case 'loopback-webcam':
         return 'webcam';
      case 'loopback-screen':
         return 'screen';
   }
};

type Props = {
   width: number;
   height: number;

   consumer?: Consumer | null;
   className?: string;

   diagnosticsLocation?: 'bottom-right' | 'top-right';
};

export default function RenderConsumerVideo({
   consumer,
   width,
   height,
   className,
   diagnosticsLocation = 'bottom-right',
}: Props) {
   const connection = useWebRtc();
   const classes = useStyles();

   const videoRef = useRef<HTMLVideoElement | null>(null);
   const showDiagnostics = useSelector(selectEnableVideoOverlay);

   useEffect(() => {
      if (connection && consumer) {
         const source: ProducerSource = consumer.appData.source;
         const device = getProducerDevice(source);

         const scalability = consumer.rtpParameters.encodings?.[0].scalabilityMode;
         const scalabilityMode = parseScalabilityMode(scalability);

         connection
            .setConsumerLayers({
               consumerId: consumer.id,
               layers: { spatialLayer: computePrefferredLayerForDevice(height, scalabilityMode, device) },
            })
            .catch((err) => console.error('Error setting prefferred layers', err));
      }
   }, [height, connection, consumer]);

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
         <video ref={videoRef} className={clsx(className, classes.video)} hidden={!isActive} autoPlay />
         {consumer && showDiagnostics && (
            <div
               className={clsx({
                  [classes.consumerInfoBottomRight]: diagnosticsLocation === 'bottom-right',
                  [classes.consumerInfoTopRight]: diagnosticsLocation === 'top-right',
               })}
            >
               <ConsumerDiagnosticInfo consumer={consumer} tileWidth={width} tileHeight={height} />
            </div>
         )}
      </>
   );
}
