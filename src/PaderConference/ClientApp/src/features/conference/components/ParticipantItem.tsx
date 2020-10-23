import { ButtonBase, ClickAwayListener, Grow, makeStyles, Paper, Popper, Typography } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect, useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import { Roles } from 'src/consts';
import { RootState } from 'src/store';
import { ParticipantDto } from 'src/store/conference-signal/types';
import MicOffIcon from '@material-ui/icons/MicOff';
import MicIcon from '@material-ui/icons/Mic';
import { getParticipantAudioLevel, getParticipantProducers } from 'src/features/media/selectors';
import ToggleIcon from './ToggleIcon';
import { motion } from 'framer-motion';
import useConsumer from 'src/store/webrtc/useConsumer';
import useSoupManager from 'src/store/webrtc/useSoupManager';

const interpolateVolume = (volume: number) => {
   volume = Math.abs(volume);
   if (volume > 70) return 0; // very quiet
   if (volume < 30) return 1; // very loud

   return 1 - (volume - 30) / 40;
};

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
   const audioLevel = useSelector((state: RootState) => getParticipantAudioLevel(state, participant?.participantId));

   const soupManager = useSoupManager();

   const consumer = useConsumer(soupManager, participant?.participantId, 'audio');

   useEffect(() => {
      console.log('consumer ', consumer);
   }, [consumer]);

   const [popperOpen, setPopperOpen] = useState(false);
   const buttonRef = useRef<HTMLButtonElement>(null);

   const audioElem = useRef<HTMLAudioElement>(null);

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
            animate={{
               backgroundColor: `rgba(41, 128, 185, ${
                  audioLevel === undefined ? 0 : interpolateVolume(audioLevel) * 0.4
               })`,
            }}
         >
            <Typography color={participant?.role === Roles.Moderator ? 'secondary' : undefined} variant="subtitle1">
               {participant ? participant?.displayName : <Skeleton />}
            </Typography>
            <ToggleIcon IconEnable={MicIcon} IconDisable={MicOffIcon} enabled={producers?.mic?.paused} />
         </ButtonBase>
         <audio ref={audioElem} autoPlay playsInline muted={false} controls={false} />
         <Popper open={popperOpen} anchorEl={buttonRef.current} transition disablePortal placement="right">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps}>
                  <Paper>
                     <ClickAwayListener onClickAway={handleClose}>
                        <Typography>Hello</Typography>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </div>
   );
}
