import { RtpEncodingParameters } from 'mediasoup-client/lib/types';
import { useState } from 'react';
import useMedia, { UseMediaState } from './useMedia';

type UseScreenState = UseMediaState & {
   stream: MediaStream | null;
};

// Each encoding represents a “spatial layer”. Entries in encodings must be ordered from lowest to highest resolution
// (encodings[0] means “spatial layer 0” while encodings[N-1] means “spatial layer N-1”, being N the number of simulcast streams).
const SCREEN_VIDEO_SIMULCAST_ENCODINGS: RtpEncodingParameters[] = [
   {
      rid: 'm',
      maxBitrate: 3000 * 1000 /** 3,000 kbps */,
      scaleResolutionDownBy: 2 /** scalabilityMode: 'S1T1' (default) */,
   },
   {
      rid: 'h',
      maxBitrate: 4300 * 1000 /** 4,000 kbps */,
      scaleResolutionDownBy: 1 /** scalabilityMode: 'S1T1' (default) */,
   },
];

export default function useScreen(): UseScreenState {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getScreen = async () => {
      const constraints: MediaStreamConstraints = { video: { height: { ideal: 1080 }, frameRate: 25 } };
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia(constraints)) as MediaStream;
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const result = useMedia('screen', getScreen, { encodings: SCREEN_VIDEO_SIMULCAST_ENCODINGS });
   return { ...result, stream };
}
