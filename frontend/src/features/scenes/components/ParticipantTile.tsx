import { fade, makeStyles, Typography, useTheme } from '@material-ui/core';
import clsx from 'classnames';
import { AnimateSharedLayout, motion, MotionValue, useTransform } from 'framer-motion';
import React, { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import { ParticipantDto } from 'src/features/conference/types';
import { useParticipantAudio } from 'src/features/media/components/ParticipantMicManager';
import { selectParticipantProducers } from 'src/features/media/selectors';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { Size } from 'src/types';

const useStyles = makeStyles((theme) => ({
   root: {
      width: '100%',
      height: '100%',
      borderRadius: theme.shape.borderRadius,
      backgroundColor: theme.palette.background.paper,
      boxShadow: theme.shadows[6],
      position: 'relative',
   },
   video: {
      position: 'absolute',
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: '100%',
      height: '100%',
      borderRadius: theme.shape.borderRadius,
      objectFit: 'cover',
   },
   infoBox: {
      position: 'absolute',
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'row',
      backgroundColor: fade(theme.palette.background.paper, 0.5),
      padding: theme.spacing(0, 1),
      left: 0,
      bottom: theme.spacing(1),
   },
   displayName: {
      marginLeft: theme.spacing(1),
   },
   centerText: {
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      width: '100%',
      height: '100%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   volumeBorder: {
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      position: 'absolute',
      backgroundColor: 'transparent',
      borderRadius: theme.shape.borderRadius,
      borderStyle: 'solid',
      borderColor: theme.palette.primary.main,
      borderWidth: 0,
   },
}));

type Props = {
   className?: string;
   participant: ParticipantDto;
   size: Size;
};

export default function ParticipantTile({ className, participant }: Props) {
   const classes = useStyles();
   const consumer = useConsumer(participant.participantId, 'webcam');
   const videoRef = useRef<HTMLVideoElement | null>(null);
   const producers = useSelector((state: RootState) => selectParticipantProducers(state, participant?.participantId));
   const isWebcamActive = consumer?.paused === false;

   const audioInfo = useParticipantAudio(participant.participantId);

   useEffect(() => {
      if (consumer?.track) {
         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         if (videoRef.current) {
            videoRef.current.srcObject = stream;
         }
      }
   }, [consumer]);

   const theme = useTheme();

   const asd = useTransform(audioInfo?.audioLevel ?? new MotionValue(0), [0, 1], [0, 10]);

   return (
      <motion.div whileHover={{ scale: 1.05, zIndex: 500 }} className={clsx(classes.root, className)}>
         <video ref={videoRef} className={classes.video} hidden={!isWebcamActive} autoPlay />
         <motion.div style={{ borderWidth: asd }} className={classes.volumeBorder} />
         {isWebcamActive ? (
            <motion.div className={classes.infoBox}>
               <AnimatedMicIcon activated={producers?.mic?.paused === false} disabledColor={theme.palette.error.main} />
               <Typography
                  component={motion.h4}
                  layoutId={`name-${participant.participantId}`}
                  variant="h4"
                  className={classes.displayName}
                  style={{ fontSize: 24 }}
               >
                  {participant.displayName}
               </Typography>
            </motion.div>
         ) : (
            <div className={classes.centerText}>
               <Typography component={motion.h4} layoutId={`name-${participant.participantId}`} variant="h4">
                  {participant.displayName}
               </Typography>
            </div>
         )}
      </motion.div>
   );
}
