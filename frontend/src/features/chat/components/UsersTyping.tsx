import { makeStyles, Typography } from '@material-ui/core';
import { AnimatePresence, motion } from 'framer-motion';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { ParticipantDto } from 'src/features/conference/types';
import UsersTypingParticipants from './UsersTypingParticipants';

const useStyles = makeStyles(() => ({
   defaultHeightSpan: {
      '&::before': {
         content: '"\u200b"',
      },
   },
}));

type Props = {
   participantsTyping?: string[];
   participantColors: { [id: string]: string };
};

export default function UsersTyping({ participantsTyping, participantColors }: Props) {
   const participants = useSelector(selectParticipants);
   const classes = useStyles();

   return (
      <AnimatePresence>
         {participantsTyping && participantsTyping.length > 0 && (
            <motion.div
               initial={{ height: 0 }}
               animate={{ height: 'auto' }}
               exit={{ height: 0 }}
               style={{ overflowY: 'hidden' }}
            >
               <Typography variant="caption" className={classes.defaultHeightSpan}>
                  <UsersTypingParticipants
                     participantColors={participantColors}
                     participants={participantsTyping
                        ?.map((x) => participants?.find((p) => p.participantId === x))
                        .filter((x) => x)
                        .map((x) => x as ParticipantDto)}
                  />
               </Typography>{' '}
            </motion.div>
         )}
      </AnimatePresence>
   );
}
