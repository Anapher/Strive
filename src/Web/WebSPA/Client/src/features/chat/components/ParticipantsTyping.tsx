import { makeStyles, Typography } from '@material-ui/core';
import { AnimatePresence, motion } from 'framer-motion';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import ParticipantsTypingText from './ParticipantsTypingText';

const useStyles = makeStyles(() => ({
   defaultHeightSpan: {
      '&::before': {
         content: '"\u200b"',
      },
   },
}));

type Props = {
   participantsTyping: string[];
   participantColors: { [id: string]: string };
};

export default function ParticipantsTyping({ participantsTyping, participantColors }: Props) {
   const participants = useSelector(selectParticipants);
   const classes = useStyles();

   const mappedParticipantsTyping = participantsTyping
      .map((id) => participants[id])
      .filter((x): x is Participant => !!x);

   return (
      <AnimatePresence>
         {participantsTyping && participantsTyping.length > 0 && (
            <motion.div
               initial={{ height: 0, marginTop: 0 }}
               animate={{ height: 'auto', marginTop: 8 }}
               exit={{ height: 0, marginTop: 0 }}
               style={{ overflowY: 'hidden', marginLeft: 8, marginRight: 8 }}
            >
               <Typography variant="caption" className={classes.defaultHeightSpan}>
                  <ParticipantsTypingText
                     participantColors={participantColors}
                     participants={mappedParticipantsTyping}
                  />
               </Typography>{' '}
            </motion.div>
         )}
      </AnimatePresence>
   );
}
