import { makeStyles, Typography } from '@material-ui/core';
import { motion, useMotionTemplate } from 'framer-motion';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import ErrorWrapper from 'src/components/ErrorWrapper';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import useConsumerMediaStream from 'src/hooks/useConsumerMediaStream';
import useMediaStreamMotionAudioLevel from 'src/hooks/useMediaStreamMotionAudioLevel';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import AudioRecorderTest from './AudioRecorderTest';

const useStyles = makeStyles((theme) => ({
   audioBarContainer: {
      position: 'relative',
      backgroundColor: theme.palette.background.default,
      height: theme.spacing(1),
      borderRadius: theme.shape.borderRadius,
   },
   audioBar: {
      position: 'absolute',
      top: 0,
      right: 0,
      bottom: 0,
      left: 0,
      borderRadius: theme.shape.borderRadius,
   },
   audioRecorder: {
      marginTop: theme.spacing(3),
   },
}));

export default function AudioSettingsTest() {
   const classes = useStyles();
   const { t } = useTranslation();

   const myId = useMyParticipantId();
   const consumer = useConsumer(myId, 'loopback-mic');

   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain, true);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('loopback-mic', localMic, audioDevice);

   const [error, setError] = useState<string | null>(null);

   const handleEnableMic = async () => {
      try {
         await micController.enable();
      } catch (error) {
         if (error && error.toString().startsWith('NotAllowedError')) {
            setError(t('conference.settings.audio.permission_denied'));
         } else {
            setError(error?.toString() ?? 'Unknown error');
         }
      }
   };

   useEffect(() => {
      if (!micController.enabled) {
         // the mic is automatically disabled on component unmount
         handleEnableMic();
      }
   }, [micController.enabled, audioDevice]);

   const stream = useConsumerMediaStream(consumer);
   const currentAudioLevel = useMediaStreamMotionAudioLevel(stream);
   const audioColor = useMotionTemplate`rgba(39, 174, 96, ${currentAudioLevel})`;

   return (
      <ErrorWrapper failed={!!error} error={error} onRetry={handleEnableMic}>
         <div>
            <Typography variant="h6" gutterBottom>
               {t('conference.settings.test')}
            </Typography>
            <Typography gutterBottom>{t('conference.settings.audio.test_description')}</Typography>
            <Typography variant="subtitle2" gutterBottom>
               {t('conference.settings.audio.audio_input_level')}
            </Typography>
            <div className={classes.audioBarContainer}>
               <motion.div className={classes.audioBar} style={{ backgroundColor: audioColor }} />
            </div>
            <AudioRecorderTest stream={stream} className={classes.audioRecorder} />
         </div>
      </ErrorWrapper>
   );
}
