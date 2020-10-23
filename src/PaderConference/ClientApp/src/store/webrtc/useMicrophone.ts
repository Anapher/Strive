import { SoupManager } from './SoupManager';
import { useMedia, UseMediaState } from './useMedia';

export function useMicrophone(getSoupManager: () => SoupManager | undefined): UseMediaState {
   const getMic = async () => {
      if (!getSoupManager()?.device.canProduce('audio')) {
         throw new Error('Cannot produce audio');
      }

      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      return stream.getAudioTracks()[0];
   };

   return useMedia(getMic, () => getSoupManager()?.sendTransport);
}
