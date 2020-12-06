import { Box, makeStyles } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { selectAvailableInputDevices } from '../selectors';
import { setCurrentDevice } from '../settingsSlice';
import DeviceSelector from './DeviceSelector';
import WebcamSettingsTest from './WebcamSettingsTest';

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
            <WebcamSettingsTest />
         </Box>
      </div>
   );
}
