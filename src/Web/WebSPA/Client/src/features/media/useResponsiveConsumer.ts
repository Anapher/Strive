import { parseScalabilityMode } from 'mediasoup-client';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import { ScalabilityMode } from 'mediasoup-client/lib/scalabilityModes';
import { RefObject, useEffect, useRef } from 'react';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import { layerResolutions as screenResolutions } from 'src/store/webrtc/hooks/useScreen';
import { layerResolutions as webcamResolutions } from 'src/store/webrtc/hooks/useWebcam';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';
import { ProducerDevice, ProducerSource } from 'src/store/webrtc/types';
import { Size } from 'src/types';

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

const computePrefferredLayerForResolutions = (
   height: number,
   scalabilityMode: ScalabilityMode,
   resolutions: number[],
) => {
   for (let i = 0; i < Math.min(scalabilityMode.spatialLayers, resolutions.length); i++) {
      const resolution = resolutions[i];

      if (resolution * 1.1 < height) continue; // allow scaling to a maximum of 10%
      return i;
   }

   return scalabilityMode.spatialLayers - 1;
};

/**
 * Takes care of adjusting the requested scalability layers depending on the element size
 * @param ref the element ref that renders the consumer video
 * @param consumer the consumer
 */
export default function useResponsiveConsumer<T extends HTMLElement>(
   ref: RefObject<T> | T | null | undefined,
   consumer: Consumer | undefined | null,
) {
   const connection = useWebRtc();
   const lastSize = useRef<Size | undefined>();

   const onUpdate = ({ height }: Size) => {
      if (connection && consumer) {
         const source: ProducerSource = consumer.appData.source;
         const device = getProducerDevice(source);

         const scalability = consumer.rtpParameters.encodings?.[0].scalabilityMode;
         const scalabilityMode = parseScalabilityMode(scalability);

         // this call takes care of not sending a new request if the computed scalability is already applied
         connection
            .setConsumerLayers({
               consumerId: consumer.id,
               layers: { spatialLayer: computePrefferredLayerForDevice(height, scalabilityMode, device) },
            })
            .catch((err) => console.error('Error setting preferred layers', err));
      }
   };

   const onResize = (size: Size | undefined) => {
      if (size) {
         onUpdate(size);
         lastSize.current = size;
      }
   };

   useEffect(() => {
      if (lastSize.current) {
         onUpdate(lastSize.current);
      }
   }, [consumer, connection]);

   useThrottledResizeObserver(1000, { onResize, ref });
}
