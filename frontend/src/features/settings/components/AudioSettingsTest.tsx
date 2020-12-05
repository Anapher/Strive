import { Box, Button, makeStyles, Typography } from '@material-ui/core';
import { motion, useMotionTemplate, useMotionValue, useTransform } from 'framer-motion';
import hark from 'hark';
import React, { useEffect, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import { selectMyParticipantId } from 'src/features/auth/selectors';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import { RootState } from 'src/store';
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

   const myId = useSelector(selectMyParticipantId);
   const consumer = useConsumer(myId, 'loopback-mic');
   const audioLevel = useMotionValue(0);

   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain, true);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('loopback-mic', localMic, audioDevice);

   useEffect(() => {
      // the mic is automatically disabled on component unmount
      micController.enable();
   }, []);

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
   const audioColor = useMotionTemplate`rgba(39, 174, 96, ${transform})`;

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
            setRecordingState(true);
            mediaRecorder.current.start();
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
      <div>
         <audio ref={audioRef} muted /> {/* required, else the analyser wont work */}
         <audio ref={playbackAudioElem} onEnded={handlePlaybackEnded} />
         <Typography variant="h6" gutterBottom>
            Test
         </Typography>
         <Typography gutterBottom>
            We will loopback your current audio input and show the results here. This way, you can check what other
            participants actually receive and you also get a taste of the actual delay.
         </Typography>
         <div className={classes.audioBarContainer}>
            <motion.div className={classes.audioBar} style={{ backgroundColor: audioColor }} />
         </div>
         <Box mt={3}>
            <Typography gutterBottom>
               Click the button below and say something. Click it again and we will replay what you just said.
            </Typography>
            <Box display="flex" alignItems="flex-start">
               <Button variant="contained" color="secondary" onClick={handleToggleRecordAudio} style={{ width: 200 }}>
                  {recordingState === true
                     ? 'Recording...'
                     : recordingState === false
                     ? 'Let me hear myself'
                     : 'Playing back...'}
               </Button>
               <Box ml={2}>
                  <Typography variant="caption">Something to say:</Typography>
                  <Typography variant="subtitle2">{`"PaderConference is much better than other video conference services"`}</Typography>
               </Box>
            </Box>
         </Box>
      </div>
   );
}
