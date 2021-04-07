import debug from 'debug';
import { ProducerOptions, RtpEncodingParameters } from 'mediasoup-client/lib/types';
import { useState } from 'react';
import { fetchEncodings } from '../utils';
import useMedia, { UseMediaState } from './useMedia';

const log = debug('webrtc:hooks:useWebcam');

type UseWebcam = UseMediaState & {
   stream: MediaStream | null;
};
export const layerResolutions = [180, 360, 720];
const requestedHeight = 720;

// Each encoding represents a “spatial layer”. Entries in encodings must be ordered from lowest to highest resolution
// (encodings[0] means “spatial layer 0” while encodings[N-1] means “spatial layer N-1”, being N the number of simulcast streams).
const CAM_VIDEO_SIMULCAST_ENCODINGS: RtpEncodingParameters[] = [
   { rid: 's', maxBitrate: 200 * 1000 },
   { rid: 'm', maxBitrate: 300 * 1000 },
   { rid: 'h', maxBitrate: 3500 * 1000 },
];

export default function useWebcam(loopback = false): UseWebcam {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getCam = async (deviceId?: string) => {
      log('Request webcam with height=%d', requestedHeight);

      const constraints: MediaStreamConstraints = {
         video: { height: { ideal: requestedHeight }, aspectRatio: { ideal: 16 / 9 }, frameRate: 25, deviceId },
      };
      const stream = await navigator.mediaDevices.getUserMedia(constraints);
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const getOptions: (track: MediaStreamTrack) => Partial<ProducerOptions> = (track) => {
      const settings = track.getSettings();
      log('Got video with %d x %d', settings.width, settings.height);

      const scaledLayers = fetchEncodings(
         settings.height ?? requestedHeight,
         layerResolutions,
         CAM_VIDEO_SIMULCAST_ENCODINGS,
      );
      log('Computed layer encodings: %O', scaledLayers);

      return { encodings: scaledLayers };
   };

   const result = useMedia(loopback ? 'loopback-webcam' : 'webcam', getCam, getOptions);
   return { ...result, stream };
}
