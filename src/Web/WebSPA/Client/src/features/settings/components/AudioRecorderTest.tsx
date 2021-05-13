import { Box, Button, Typography } from '@material-ui/core';
import React, { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { showMessage } from 'src/store/notifier/actions';

type Props = {
   stream?: MediaStream | null;
   className?: string;
};

export default function AudioRecorderTest({ stream, className }: Props) {
   if (!window.MediaRecorder) {
      return null;
   }

   const dispatch = useDispatch();
   const { t } = useTranslation();

   const [recordingState, setRecordingState] = useState<boolean | 'playing'>(false);
   const playbackAudioElem = useRef<HTMLAudioElement>(null);
   const mediaRecorder = useRef<MediaRecorder | undefined>();
   const recordedChunks = useRef<Blob[]>([]);

   useEffect(() => {
      if (stream) {
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
            if (recorder.state !== 'inactive') {
               recorder.stop();
            }
         };
      }
   }, [stream]);

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
      <div className={className}>
         <audio ref={playbackAudioElem} onEnded={handlePlaybackEnded} />

         <Typography gutterBottom>{t('conference.settings.audio.repeat_audio_description')}</Typography>
         <Box display="flex" alignItems="flex-start">
            <Button
               variant="contained"
               color="secondary"
               onClick={handleToggleRecordAudio}
               style={{ maxWidth: 260 }}
               fullWidth
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
      </div>
   );
}
