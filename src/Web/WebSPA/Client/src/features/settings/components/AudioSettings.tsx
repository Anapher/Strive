import { Box, makeStyles, Mark, Slider, Typography } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import ErrorWrapper from 'src/components/ErrorWrapper';
import { RootState } from 'src/store';
import useWebRtcStatus from 'src/store/webrtc/hooks/useWebRtcStatus';
import { setAudioGain, setCurrentDevice } from '../reducer';
import { selectAvailableInputDevices } from '../selectors';
import AudioSettingsTest from './AudioSettingsTest';
import DeviceSelector from './DeviceSelector';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      padding: theme.spacing(3),
      paddingTop: 0,
   },
   slider: {
      padding: theme.spacing(0, 2),
   },
}));

const marks: Mark[] = [
   { value: 1, label: '0%' },
   { value: 0, label: '-100%' },
   { value: 2, label: '100%' },
   { value: 3, label: '200%' },
];

export default function AudioSettings() {
   const classes = useStyles();
   const { t } = useTranslation();
   const audioGain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const dispatch = useDispatch();

   const handleChangeGain = (_: React.ChangeEvent<unknown>, value: number | number[]) => {
      dispatch(setAudioGain(value as number));
   };

   const audioDevices = useSelector((state: RootState) => selectAvailableInputDevices(state, 'mic'));
   const webrtcState = useWebRtcStatus();

   return (
      <div className={classes.root}>
         <DeviceSelector
            devices={audioDevices}
            label={t('common:microphone')}
            defaultName={t('common:microphone')}
            selectedDevice={audioDevice}
            onChange={(device) => dispatch(setCurrentDevice({ device, source: 'mic' }))}
         />
         <Box mt={4}>
            <Typography variant="h6" gutterBottom>
               {t('conference.settings.audio.gain')}
            </Typography>
            <div className={classes.slider}>
               <Slider
                  value={audioGain}
                  min={0}
                  max={3}
                  onChange={handleChangeGain}
                  step={0.2}
                  valueLabelDisplay="auto"
                  marks={marks}
                  valueLabelFormat={(val) => `${Math.round((val - 1) * 100)}%`}
               />
            </div>
         </Box>
         <Box mt={4}>
            <ErrorWrapper
               failed={webrtcState !== 'connected'}
               error={t('conference.settings.webrtc_not_connected_error')}
            >
               <AudioSettingsTest />
            </ErrorWrapper>
         </Box>
      </div>
   );
}
