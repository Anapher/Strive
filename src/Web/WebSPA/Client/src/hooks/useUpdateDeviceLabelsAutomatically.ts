import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectLocalDevices } from 'src/features/settings/selectors';
import { fetchDevices } from 'src/features/settings/thunks';
import useWebRtc from 'src/store/webrtc/hooks/useWebRtc';

/**
 * If the user did not give permissions yet, the devices fetched by enumerateDevices() will all have a undefined label.
 * That is fine, we do not want to force the user to give us permissions if he does not actually use any device.
 * At the moment he actually enables his microphone/webcam, he has to give permissions and therefore we will be able
 * to query the actual labels of the devices. This hook takes care of that process, it reacts to the event that a device
 * is enabled and then updates the devices.
 */
const useUpdateDeviceLabelsAutomatically = () => {
   const dispatch = useDispatch();
   const connection = useWebRtc();
   const devices = useSelector(selectLocalDevices);

   useEffect(() => {
      if (!connection || !devices) return;

      if (devices.find((x) => !x.label)) {
         const handleDeviceEnabled = () => {
            dispatch(fetchDevices());
         };

         connection.on('onDeviceEnabled', handleDeviceEnabled);
         return () => {
            connection.off('onDeviceEnabled', handleDeviceEnabled);
         };
      }
   }, [devices, connection, dispatch]);
};

export default useUpdateDeviceLabelsAutomatically;
