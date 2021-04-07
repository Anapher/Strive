import { ProducerOptions, RtpEncodingParameters } from 'mediasoup-client/lib/types';
import { useState } from 'react';
import useMedia, { UseMediaState } from './useMedia';
import debug from 'debug';
import { fetchEncodings } from '../utils';

const log = debug('webrtc:hooks:useScreen');

type UseScreenState = UseMediaState & {
   stream: MediaStream | null;
};

export const layerResolutions = [1080];
const requestedHeight = 1080;

// Each encoding represents a “spatial layer”. Entries in encodings must be ordered from lowest to highest resolution
// (encodings[0] means “spatial layer 0” while encodings[N-1] means “spatial layer N-1”, being N the number of simulcast streams).
const SCREEN_VIDEO_SIMULCAST_ENCODINGS: RtpEncodingParameters[] = [
   {
      rid: 'h',
   },
];

export default function useScreen(): UseScreenState {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getScreen = async () => {
      log('Request screen with height=%d', requestedHeight);
      const constraints: MediaStreamConstraints = { video: { height: { ideal: requestedHeight }, frameRate: 25 } };
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia(constraints)) as MediaStream;
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const getOptions: (track: MediaStreamTrack) => Partial<ProducerOptions> = (track) => {
      const settings = track.getSettings();
      log('Got video with %d x %d', settings.width, settings.height);

      const scaledLayers = fetchEncodings(
         settings.height ?? requestedHeight,
         layerResolutions,
         SCREEN_VIDEO_SIMULCAST_ENCODINGS,
      );
      log('Computed layer encodings: %O', scaledLayers);

      return { encodings: scaledLayers };
   };

   const result = useMedia('screen', getScreen, getOptions);
   return { ...result, stream };
}
