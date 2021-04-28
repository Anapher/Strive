import { List, ListItem, ListItemText, Typography } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { selectTalkingStickQueue } from '../../selectors';

type Props = {
   onPassStick: (participantId: string) => void;
};
export default function PassStickList({ onPassStick }: Props) {
   const queue = useSelector(selectTalkingStickQueue);
   const participants = useSelector(selectParticipants);

   if (queue.length === 0) {
      return (
         <Typography variant="body2" align="center" style={{ marginTop: 8 }}>
            At the moment, no participants want to say something...
         </Typography>
      );
   }

   return (
      <List dense>
         {queue
            .map((id) => participants.find((x) => x.id === id))
            .filter((x): x is Participant => !!x)
            .map(({ id, displayName }) => (
               <ListItem key={id} button onClick={() => onPassStick(id)}>
                  <ListItemText primary={displayName} />
               </ListItem>
            ))}
      </List>
   );
}
