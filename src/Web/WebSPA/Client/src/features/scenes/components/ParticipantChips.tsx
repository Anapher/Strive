import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { AnimatePresence, motion, MotionProps } from 'framer-motion';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectParticipantAudio } from 'src/features/media/selectors';
import ParticipantInfoChip from './ParticipantInfoChip';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      justifyContent: 'flex-end',
      width: '100%',
   },
   chip: {
      marginLeft: theme.spacing(1),
   },
}));

const chipMotion: MotionProps = {
   initial: { opacity: 0 },
   animate: { opacity: 1 },
   exit: { opacity: 0 },
   transition: {
      mass: 0.2,
      power: 1,
   },
};

type Props = {
   participantIds: string[];
   className?: string;
};

export default function ParticipantChips({ participantIds, className }: Props) {
   const participants = useSelector(selectParticipants);
   const audioInfo = useSelector(selectParticipantAudio);
   const classes = useStyles();

   return (
      <div className={clsx(className, classes.root)}>
         <AnimatePresence>
            {participantIds.map((participantId, i) => (
               <ParticipantInfoChip
                  key={participantId}
                  component={motion.div}
                  className={i === 0 ? undefined : classes.chip}
                  participantId={participantId}
                  audioInfo={audioInfo[participantId]}
                  participant={participants[participantId]}
                  {...(chipMotion as any)}
               />
            ))}
         </AnimatePresence>
      </div>
   );
}
