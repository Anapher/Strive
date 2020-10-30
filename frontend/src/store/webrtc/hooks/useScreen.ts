import { useState } from 'react';
import useMedia, { UseMediaState } from './useMedia';

type UseScreenState = UseMediaState & {
   stream: MediaStream | null;
};

export function useScreen(): UseScreenState {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getScreen = async () => {
      const constraints: MediaStreamConstraints = { video: { height: { ideal: 720 }, frameRate: 25 } };
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia(constraints)) as MediaStream;
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const result = useMedia('screen', getScreen);
   return { ...result, stream };
}
