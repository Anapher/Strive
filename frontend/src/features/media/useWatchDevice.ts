import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ProducerSource } from 'src/store/webrtc/types';
import { showMessage } from '../notifier/actions';
import { setCurrentDevice } from '../settings/settingsSlice';

export default function useWatchSelectedDevice(source: ProducerSource) {
   const equipment = useSelector((state: RootState) => state.media.equipment);
   const currentDevice = useSelector((state: RootState) => state.settings.obj[source].device);
   const devices = useSelector((state: RootState) => state.settings.availableDevices);
   const dispatch = useDispatch();

   useEffect(() => {
      if (currentDevice?.type === 'equipment') {
         if (!equipment?.find((x) => x.equipmentId === currentDevice.equipmentId)) {
            const device = devices?.find((x) => x.source === source);
            dispatch(
               setCurrentDevice({ source, device: device ? { type: 'local', deviceId: device.deviceId } : undefined }),
            );

            dispatch(
               showMessage({
                  variant: 'warning',
                  message: device
                     ? `Equipment disconnected, use ${
                          device.label || 'a different microphone'
                       } as new device for ${source}.`
                     : `Equipment disconnected and no alternative devices available. The device for ${source} was removed.`,
               }),
            );
         }
      }
   }, [equipment]);
}
