import { Box, Button, makeStyles, Typography } from '@material-ui/core';
import { motion, useMotionTemplate, useMotionValue, useSpring, useTransform } from 'framer-motion';
import hark from 'hark';
import React, { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import ErrorWrapper from 'src/components/ErrorWrapper';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import { showMessage } from 'src/store/notifier/actions';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';

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
}));

export default function AudioSettingsTest() {
   const classes = useStyles();
   const { t } = useTranslation();
   const dispatch = useDispatch();

   const myId = useMyParticipantId();
   const consumer = useConsumer(myId, 'loopback-mic');
   const audioLevel = useMotionValue(0);

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

   const audioRef = useRef<HTMLAudioElement>(null);
   const mediaRecorder = useRef<MediaRecorder | undefined>();
   const recordedChunks = useRef<Blob[]>([]);

   useEffect(() => {
      if (consumer) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         const analyser = hark(stream, { play: false });
         analyser.on('volume_change', (dBs) => {
            // The exact formula to convert from dBs (-100..0) to linear (0..1) is:
            //   Math.pow(10, dBs / 20)
            // However it does not produce a visually useful output, so let exagerate
            // it a bit. Also, let convert it from 0..1 to 0..10 and avoid value 1 to
            // minimize component renderings.
            let audioVolume = Math.round(Math.pow(10, dBs / 85) * 10);

            if (audioVolume === 1) audioVolume = 0;
            audioLevel.set(audioVolume / 10);
         });

         if (audioRef.current) {
            audioRef.current.srcObject = stream;
            audioRef.current.play();
         }

         const recorder = new MediaRecorder(stream);
         recorder.ondataavailable = (e) => recordedChunks.current.push(e.data);
         recorder.onstop = () => {
            if (!playbackAudioElem.current) return;
            const blob = new Blob(recordedChunks.current, { type: 'audio/ogg; codecs=opus' });

            const audioURL = window.URL.createObjectURL(blob);
            playbackAudioElem.current.src = audioURL;
            playbackAudioElem.current.play();
         };

         mediaRecorder.current = recorder;

         return () => {
            analyser.stop();

            if (recorder.state !== 'inactive') recorder.stop();
         };
      }
   }, [consumer]);

   const transform = useTransform(audioLevel, [0, 1], [0, 2]);
   const currentAudioLevel = useSpring(transform);
   const audioColor = useMotionTemplate`rgba(39, 174, 96, ${currentAudioLevel})`;

   const [recordingState, setRecordingState] = useState<boolean | 'playing'>(false);
   const playbackAudioElem = useRef<HTMLAudioElement>(null);

   const handleToggleRecordAudio = () => {
      if (!mediaRecorder.current) return;
      if (!playbackAudioElem.current) return;

      switch (recordingState) {
         case true:
            mediaRecorder.current.stop();
            setRecordingState('playing');
            break;
         case false:
            recordedChunks.current = []; // clear chunks
            try {
               mediaRecorder.current.start();
            } catch (error) {
               console.error(error);
               dispatch(showMessage({ type: 'error', message: `An error occurred starting the media recorder` }));
            }
            setRecordingState(true);
            break;
         case 'playing':
            playbackAudioElem.current.src = '';
            setRecordingState(false);
            break;
      }
   };

   const handlePlaybackEnded = () => {
      setRecordingState(false);
   };

   return (
      <ErrorWrapper failed={!!error} error={error} onRetry={handleEnableMic}>
         <div>
            <audio ref={audioRef} muted /> {/* required, else the analyser wont work */}
            <audio ref={playbackAudioElem} onEnded={handlePlaybackEnded} />
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
            <Box mt={3}>
               <Typography gutterBottom>{t('conference.settings.audio.repeat_audio_description')}</Typography>
               <Box display="flex" alignItems="flex-start">
                  <Button
                     variant="contained"
                     color="secondary"
                     onClick={handleToggleRecordAudio}
                     style={{ width: 200 }}
                  >
                     {recordingState === true
                        ? t('conference.settings.audio.repeat_recording')
                        : recordingState === false
                        ? t('conference.settings.audio.repeat_idle')
                        : t('conference.settings.audio.repeat_playing')}
                  </Button>
                  <Box ml={2}>
                     <Typography variant="caption">{t('conference.settings.audio.something_to_say')}</Typography>
                     <Typography variant="subtitle2">{t('conference.settings.audio.something_to_say_text')}</Typography>
                  </Box>
               </Box>
            </Box>
         </div>
      </ErrorWrapper>
   );
}
