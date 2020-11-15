import {
   Box,
   ButtonBase,
   ClickAwayListener,
   Grow,
   makeStyles,
   Paper,
   Popper,
   Typography,
   useTheme,
} from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
import { motion, useMotionValue, useTransform } from 'framer-motion';
import _ from 'lodash';
import React, { useCallback, useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import IconHide from 'src/components/IconHide';
import { Roles } from 'src/consts';
import { patchParticipantAudio } from 'src/features/media/mediaSlice';
import { selectParticipantAudioInfo, selectParticipantProducers } from 'src/features/media/selectors';
import { RootState } from 'src/store';
import { ParticipantDto } from '../types';
import ParticipantItemPopper from './ParticipantItemPopper';

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
   const producers = useSelector((state: RootState) => selectParticipantProducers(state, participant?.participantId));

   const dispatch = useDispatch();
   const audioVol = useMotionValue(0);
   const audioVolBackground = useTransform(audioVol, (value) => `rgba(41, 128, 185, ${value})`);
   const audioInfo = useSelector((state: RootState) => selectParticipantAudioInfo(state, participant?.participantId));

   const theme = useTheme();

   const handleChangeMuted = (muted: boolean) => {
      if (participant) dispatch(patchParticipantAudio({ participantId: participant.participantId, data: { muted } }));
   };

   const handleChangeVolume = useCallback(
      _.throttle((volume: number) => {
         if (participant)
            dispatch(patchParticipantAudio({ participantId: participant.participantId, data: { volume } }));
      }, 200),
      [participant],
   );

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
            <IconHide hidden={!producers?.mic}>
               <AnimatedMicIcon activated={!producers?.mic?.paused} disabledColor={theme.palette.error.main} />
            </IconHide>
         </ButtonBase>
         {participant && (
            <Popper open={popperOpen} anchorEl={buttonRef.current} transition placement="right-start">
               {({ TransitionProps }) => (
                  <Grow {...TransitionProps} style={{ transformOrigin: 'left top' }}>
                     <Paper style={{ width: 400 }}>
                        <ClickAwayListener onClickAway={handleClose}>
                           <Box p={2}>
                              <ParticipantItemPopper
                                 participant={participant}
                                 audioLevel={audioVol}
                                 muted={audioInfo?.muted ?? false}
                                 onChangeMuted={handleChangeMuted}
                                 volume={audioInfo?.volume ?? 0}
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
