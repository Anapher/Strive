import { Box, ButtonBase, ClickAwayListener, Grow, makeStyles, Paper, Popper, Typography } from '@material-ui/core';
import MicIcon from '@material-ui/icons/Mic';
import MicOffIcon from '@material-ui/icons/MicOff';
import { Skeleton } from '@material-ui/lab';
import { motion, useMotionValue, useTransform } from 'framer-motion';
import hark from 'hark';
import React, { useEffect, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Roles } from 'src/consts';
import { getParticipantProducers } from 'src/features/media/selectors';
import { showMessage } from 'src/features/notifier/actions';
import { RootState } from 'src/store';
import { ParticipantDto } from 'src/store/conference-signal/types';
import useConsumer from 'src/store/webrtc/useConsumer';
import useSoupManager from 'src/store/webrtc/useSoupManager';
import ParticipantItemPopper from './ParticipantItemPopper';
import ToggleIcon from './ToggleIcon';

const useStyles = makeStyles((theme) => ({
   root: {
      marginLeft: theme.spacing(1),
   },
   button: {
      padding: theme.spacing(0, 1),
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'center',
      borderRadius: theme.shape.borderRadius,
      width: '100%',
   },
}));

type Props = {
   participant?: ParticipantDto;
};

export default function ParticipantItem({ participant }: Props) {
   const classes = useStyles();
   const producers = useSelector((state: RootState) => getParticipantProducers(state, participant?.participantId));

   const soupManager = useSoupManager();
   const dispatch = useDispatch();

   const consumer = useConsumer(soupManager, participant?.participantId, 'audio');
   const audioElem = useRef<HTMLAudioElement>(null);
   const audioVol = useMotionValue(0);
   const audioVolBackground = useTransform(audioVol, (value) => `rgba(41, 128, 185, ${value})`);
   const [muted, setMuted] = useState(false);
   const [volume, setVolume] = useState(0.75);

   useEffect(() => {
      if (consumer && audioElem.current) {
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

            audioVol.set(audioVolume / 10);
         });

         audioElem.current.srcObject = stream;
         audioElem.current.volume = volume;
         audioElem.current
            .play()
            .catch((error) => dispatch(showMessage({ message: error.toString(), variant: 'error' })));

         return () => {
            analyser.stop();
         };
      }
   }, [consumer, audioElem.current]);

   const handleChangeMuted = (mute: boolean) => {
      setMuted(mute);
   };

   const handleChangeVolume = (volume: number) => {
      setVolume(volume);

      if (audioElem.current) audioElem.current.volume = volume;
   };

   const [popperOpen, setPopperOpen] = useState(false);
   const buttonRef = useRef<HTMLButtonElement>(null);

   const handleClose = (event: React.MouseEvent<Document, MouseEvent>) => {
      if (buttonRef.current && buttonRef.current?.contains(event.target as HTMLElement)) {
         return;
      }

      setPopperOpen(false);
   };

   const handleToggle = () => {
      setPopperOpen((prevOpen) => !prevOpen);
   };

   return (
      <div className={classes.root}>
         <ButtonBase
            onClick={handleToggle}
            ref={buttonRef}
            component={motion.button}
            className={classes.button}
            style={{ backgroundColor: audioVolBackground as any }}
         >
            <Typography color={participant?.role === Roles.Moderator ? 'secondary' : undefined} variant="subtitle1">
               {participant ? participant?.displayName : <Skeleton />}
            </Typography>
            <ToggleIcon IconEnable={MicIcon} IconDisable={MicOffIcon} enabled={producers?.mic?.paused} />
         </ButtonBase>
         <audio ref={audioElem} autoPlay playsInline muted={muted} controls={false} />
         {participant && (
            <Popper open={popperOpen} anchorEl={buttonRef.current} transition placement="right-start">
               {({ TransitionProps }) => (
                  <Grow {...TransitionProps}>
                     <Paper style={{ width: 400 }}>
                        <ClickAwayListener onClickAway={handleClose}>
                           <Box p={2}>
                              <ParticipantItemPopper
                                 participant={participant}
                                 audioLevel={audioVol}
                                 muted={muted}
                                 onChangeMuted={handleChangeMuted}
                                 volume={volume}
                                 onChangeVolume={handleChangeVolume}
                              />
                           </Box>
                        </ClickAwayListener>
                     </Paper>
                  </Grow>
               )}
            </Popper>
         )}
      </div>
   );
}
