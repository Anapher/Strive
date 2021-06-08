import { fade, IconButton, makeStyles, Typography, useTheme } from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import clsx from 'classnames';
import { AnimateSharedLayout, motion, MotionValue, useTransform } from 'framer-motion';
import React, { useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import ParticipantContextMenuPopper from 'src/features/conference/components/ParticipantContextMenuPopper';
import { Participant } from 'src/features/conference/types';
import { useParticipantAudio } from 'src/features/media/components/ParticipantMicManager';
import { selectParticipantMicActivated } from 'src/features/media/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';

const useStyles = makeStyles((theme) => ({
   root: {
      position: 'relative',
      width: '100%',
      height: '100%',
      borderRadius: theme.shape.borderRadius,
      backgroundColor: theme.palette.background.paper,
      boxShadow: theme.shadows[6],
   },
   video: {
      borderRadius: theme.shape.borderRadius,
   },
   infoBox: {
      position: 'absolute',
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'row',
      left: theme.spacing(1),
      bottom: theme.spacing(1),

      backgroundColor: fade(theme.palette.background.paper, 0.5),
      padding: theme.spacing(0, 1),
      borderRadius: theme.shape.borderRadius,
   },
   micIconWithoutWebcam: {
      position: 'absolute',
      left: theme.spacing(2),
      bottom: theme.spacing(2),
   },
   centerText: {
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   volumeBorder: {
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'transparent',
      borderRadius: theme.shape.borderRadius,
      borderStyle: 'solid',
      borderColor: theme.palette.primary.main,
      borderWidth: 0,
   },
   moreButton: {
      position: 'absolute',
      right: theme.spacing(1),
      top: theme.spacing(1),
   },
}));

type Props = {
   className?: string;
   participant: Participant;
   width: number;
   height: number;
};

export default function ParticipantTile({ className, participant, width, height }: Props) {
   const classes = useStyles();
   const consumer = useConsumer(participant.id, 'webcam');
   const micActivated = useSelector((state: RootState) => selectParticipantMicActivated(state, participant?.id));
   const isWebcamActive = consumer?.paused === false;
   const myParticipantId = useMyParticipantId();

   const isMe = participant.id === myParticipantId;

   const audioInfo = useParticipantAudio(participant.id);

   const theme = useTheme();
   const audioBorder = useTransform(audioInfo?.audioLevel ?? new MotionValue(0), [0, 1], [0, 10]);

   const isSmall = width < 400;

   const [contextMenuOpen, setContextMenuOpen] = useState(false);
   const moreIconRef = useRef(null);

   const handleOpenContextMenu = () => {
      setContextMenuOpen(true);
   };

   const handleCloseContextMenu = () => {
      setContextMenuOpen(false);
   };

   return (
      <AnimateSharedLayout>
         <motion.div className={clsx(classes.root, className)}>
            <RenderConsumerVideo consumer={consumer} height={height} width={width} className={classes.video} />
            <motion.div style={{ borderWidth: audioBorder }} className={classes.volumeBorder} />

            {isWebcamActive && (
               <motion.div className={classes.infoBox}>
                  <AnimatedMicIcon activated={micActivated} disabledColor={theme.palette.error.main} />
                  <Typography
                     component={motion.h4}
                     layoutId="name"
                     variant="h4"
                     style={{ fontSize: isSmall ? 14 : 18, marginLeft: 8 }}
                  >
                     {participant.displayName}
                  </Typography>
               </motion.div>
            )}

            {!isWebcamActive && (
               <>
                  <AnimatedMicIcon
                     activated={micActivated}
                     disabledColor={theme.palette.error.main}
                     className={classes.micIconWithoutWebcam}
                  />
                  <div className={classes.centerText}>
                     <Typography
                        component={motion.span}
                        layoutId="name"
                        variant="h4"
                        style={{ fontSize: isSmall ? 16 : 24 }}
                     >
                        {participant.displayName}
                     </Typography>
                  </div>
               </>
            )}

            {!isMe && (
               <div className={classes.moreButton}>
                  <IconButton
                     ref={moreIconRef}
                     aria-label="options"
                     size={isSmall ? 'small' : 'medium'}
                     onClick={handleOpenContextMenu}
                  >
                     <MoreVertIcon />
                  </IconButton>
               </div>
            )}
         </motion.div>
         <ParticipantContextMenuPopper
            open={contextMenuOpen}
            onClose={handleCloseContextMenu}
            participant={participant}
            anchorEl={moreIconRef.current}
         />
      </AnimateSharedLayout>
   );
}
