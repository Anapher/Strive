import { makeStyles } from '@material-ui/core';
import React from 'react';
import { Participant } from 'src/features/conference/types';

const useStyles = makeStyles((theme) => ({
   participantText: {
      color: theme.palette.secondary.main,
   },
}));

type Props = {
   participants: Participant[];
   participantColors: { [id: string]: string };
};

export default function UsersTypingParticipants({ participants, participantColors }: Props) {
   const classes = useStyles();
   const beginning = participants.slice(0, 2);
   const tail = participants.slice(2);

   return (
      <>
         {beginning.map((x, i) => (
            <span key={x.id}>
               {i !== 0 && <span>{i === participants.length - 1 ? ' and ' : ', '}</span>}
               <span className={classes.participantText} style={{ color: participantColors[x.id] }}>
                  {x.displayName}
               </span>
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
