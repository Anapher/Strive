import { makeStyles, Typography } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { ParticipantDto } from 'src/features/conference/types';

const useStyles = makeStyles((theme) => ({
   participantText: {
      color: theme.palette.secondary.main,
   },
   defaultHeightSpan: {
      // '&::before': {
      //    content: '"\u200b"',
      // },
   },
}));

type Props = {
   participantsTyping?: string[];
};

export default function UsersTyping({ participantsTyping }: Props) {
   const participants = useSelector(selectParticipants);
   const classes = useStyles();

   return (
      <Typography variant="caption" className={classes.defaultHeightSpan}>
         {participantsTyping && participantsTyping.length > 0 && (
            <Participants
               participants={participantsTyping
                  ?.map((x) => participants?.find((p) => p.participantId === x))
                  .filter((x) => x)
                  .map((x) => x as ParticipantDto)}
            />
         )}
      </Typography>
   );
}

function Participants({ participants }: { participants: ParticipantDto[] }) {
   const classes = useStyles();
   const beginning = participants.slice(0, 2);
   const tail = participants.slice(2);

   return (
      <>
         {beginning.map((x, i) => (
            <span key={x.participantId}>
               {i !== 0 && <span>{i === participants.length - 1 ? ' and ' : ', '}</span>}
               <span className={classes.participantText}>{x.displayName}</span>
            </span>
         ))}
         {tail.length > 0 ? (
            <span>
               {' '}
               and <span className={classes.participantText}>{tail.length}</span> more are typing
            </span>
         ) : (
            <span>{beginning.length === 1 ? ' is typing' : ' are typing'}</span>
         )}
      </>
   );
}
