import { useState } from 'react';
import { SoupManager } from './SoupManager';
import { useMedia, UseMediaState } from './useMedia';

type UseScreenState = UseMediaState & {
   stream: MediaStream | null;
};

export function useScreen(soup?: SoupManager): UseScreenState {
   const [stream, setStream] = useState<MediaStream | null>(null);

   const getScreen = async () => {
      if (!soup) throw new Error('Soup not initialized');

      if (!soup.device.canProduce('video')) {
         throw new Error('Cannot produce video');
      }

      const constraints: MediaStreamConstraints = { video: { height: { ideal: 720 }, frameRate: 25 } };
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia(constraints)) as MediaStream;
      setStream(stream);

      return stream.getVideoTracks()[0];
   };

   const result = useMedia(getScreen, () => soup?.sendTransport);
   return { ...result, stream };
}
