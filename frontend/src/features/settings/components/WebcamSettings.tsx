import { Box, makeStyles } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { selectAvailableInputDevices } from '../selectors';
import { setCurrentDevice } from '../reducer';
import DeviceSelector from './DeviceSelector';
import WebcamSettingsTest from './WebcamSettingsTest';
import useWebRtcStatus from 'src/store/webrtc/hooks/useWebRtcStatus';
import ErrorWrapper from 'src/components/ErrorWrapper';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
   },
   slider: {
      padding: theme.spacing(0, 2),
   },
}));

export default function WebcamSettings() {
   const classes = useStyles();
   const selectedDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);
   const availableDevices = useSelector((state: RootState) => selectAvailableInputDevices(state, 'webcam'));
   const dispatch = useDispatch();
   const webrtcState = useWebRtcStatus();

   return (
      <div className={classes.root}>
         <DeviceSelector
            devices={availableDevices}
            label="Webcam"
            defaultName="Webcam"
            selectedDevice={selectedDevice}
            onChange={(device) => dispatch(setCurrentDevice({ device, source: 'webcam' }))}
         />
         <Box mt={4}>
            <ErrorWrapper
               failed={webrtcState !== 'connected'}
               error="WebRTC not connected, so you cannot test your audio device. Please refresh the page or contact the
                  server administrator."
            >
               <WebcamSettingsTest />
            </ErrorWrapper>
         </Box>
      </div>
   );
}
