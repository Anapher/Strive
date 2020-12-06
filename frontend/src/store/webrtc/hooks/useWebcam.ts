import { RtpEncodingParameters } from 'mediasoup-client/lib/types';
import { useState } from 'react';
import useMedia, { UseMediaState } from './useMedia';

type UseWebcam = UseMediaState & {
   stream: MediaStream | null;
};

// Each encoding represents a “spatial layer”. Entries in encodings must be ordered from lowest to highest resolution
// (encodings[0] means “spatial layer 0” while encodings[N-1] means “spatial layer N-1”, being N the number of simulcast streams).
const CAM_VIDEO_SIMULCAST_ENCODINGS: RtpEncodingParameters[] = [
   { rid: 'm', maxBitrate: 96000, scaleResolutionDownBy: 4 /** scalabilityMode: 'S1T1' (default) */ },
   { rid: 'h', maxBitrate: 680000, scaleResolutionDownBy: 1 /** scalabilityMode: 'S1T1' (default) */ },
];

export default function useWebcam(loopback = false): UseWebcam {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getCam = async (deviceId?: string) => {
      console.log('get cam', deviceId);

      const constraints: MediaStreamConstraints = { video: { width: { ideal: 640 }, frameRate: 25, deviceId } };
      const stream = await navigator.mediaDevices.getUserMedia(constraints);
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const result = useMedia(loopback ? 'loopback-webcam' : 'webcam', getCam, {
      encodings: CAM_VIDEO_SIMULCAST_ENCODINGS,
   });
   return { ...result, stream };
}
