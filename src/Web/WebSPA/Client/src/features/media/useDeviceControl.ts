import { UseMediaState } from './../../store/webrtc/hooks/useMedia';
import { useSelector } from 'react-redux';
import useSelectorFactory from 'src/hooks/useSelectorFactory';
import { RootState } from 'src/store';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import { selectIsDeviceAvailableFactory } from '../settings/selectors';
import useDeviceManagement from './useDeviceManagement';
import useWebcam from 'src/store/webrtc/hooks/useWebcam';
import useScreen from 'src/store/webrtc/hooks/useScreen';

type DeviceControl = {
   controller: UseMediaState;
   available: boolean;
};

export function useMicrophoneControl(): DeviceControl {
   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const controller = useDeviceManagement('mic', localMic, audioDevice);
   const available = useSelectorFactory(selectIsDeviceAvailableFactory, (state: RootState, selector) =>
      selector(state, 'mic'),
   );

   return { controller, available };
}

export function useWebcamControl(): DeviceControl {
   const webcamDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);
   const localWebcam = useWebcam();
   const controller = useDeviceManagement('webcam', localWebcam, webcamDevice);
   const available = useSelectorFactory(selectIsDeviceAvailableFactory, (state: RootState, selector) =>
      selector(state, 'webcam'),
   );

   return { controller, available };
}

export function useScreenControl(): DeviceControl {
   const screenDevice = useSelector((state: RootState) => state.settings.obj.screen.device);
   const localScreen = useScreen();
   const controller = useDeviceManagement('screen', localScreen, screenDevice);
   const available = useSelectorFactory(selectIsDeviceAvailableFactory, (state: RootState, selector) =>
      selector(state, 'screen'),
   );

   return { controller, available };
}
