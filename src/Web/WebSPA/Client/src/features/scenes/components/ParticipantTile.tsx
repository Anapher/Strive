import { IconButton, makeStyles } from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import clsx from 'classnames';
import { motion, MotionValue, useTransform } from 'framer-motion';
import React, { useRef, useState } from 'react';
import { useSelector } from 'react-redux';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import ParticipantContextMenuPopper from 'src/features/conference/components/ParticipantContextMenuPopper';
import { Participant } from 'src/features/conference/types';
import { useParticipantAudio } from 'src/features/media/components/ParticipantMicManager';
import { selectParticipantMicActivated } from 'src/features/media/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import ParticipantTileLabel from './ParticipantTileLabel';

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
   disableLayoutAnimation?: boolean;
};

export default function ParticipantTile({ className, participant, width, height, disableLayoutAnimation }: Props) {
   const classes = useStyles();
   const consumer = useConsumer(participant.id, 'webcam');
   const micActivated = useSelector((state: RootState) => selectParticipantMicActivated(state, participant?.id));
   const isWebcamActive = consumer?.paused === false;
   const myParticipantId = useMyParticipantId();

   const isMe = participant.id === myParticipantId;

   const audioInfo = useParticipantAudio(participant.id);
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
      <>
         <motion.div
            layout={!disableLayoutAnimation}
            layoutId={disableLayoutAnimation ? undefined : participant.id}
            className={clsx(classes.root, className)}
         >
            <RenderConsumerVideo consumer={consumer} height={height} width={width} className={classes.video} />
            <motion.div style={{ borderWidth: audioBorder }} className={classes.volumeBorder} />

            {consumer !== undefined && (
               <ParticipantTileLabel
                  label={participant.displayName}
                  tileWidth={width}
                  tileHeight={height}
                  micActivated={micActivated}
                  webcamActivated={isWebcamActive}
               />
            )}

            {!isMe && (
               <div className={classes.moreButton}>
                  <IconButton
                     ref={moreIconRef}
                     aria-label="options"
                     size={isSmall ? 'small' : 'medium'}
                     onClick={handleOpenContextMenu}
                  >
                     <MoreVertIcon fontSize={isSmall ? 'small' : 'default'} />
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
      </>
   );
}
