import { makeStyles } from '@material-ui/core';
import React from 'react';
import { Participant } from 'src/features/conference/types';
import { TilesSceneBarInstructions } from '../tile-frame-calculations';
import ParticipantTile from './ParticipantTile';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      width: '100%',
      flexDirection: 'column',
      marginTop: 8,
   },
   participantTilesRow: {
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'center',
   },
});

type Props = {
   instructions: TilesSceneBarInstructions;
   participants: Participant[];
   marginHorizontal: number;
};

export default function TilesBarLayoutRow({ marginHorizontal, instructions, participants }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.root} style={{ paddingLeft: marginHorizontal, paddingRight: marginHorizontal }}>
         <div className={classes.participantTilesRow}>
            {participants.slice(0, instructions.tileAmount).map((x, i) => (
               <div
                  key={x.id}
                  style={{
                     flex: 1,
                     height: instructions.tileSize.height,
                     maxWidth: instructions.tileSize.width,
                     marginLeft: i === 0 ? 0 : instructions.tileSpaceBetween,
                  }}
               >
                  <ParticipantTile {...instructions.tileSize} participant={x} />
               </div>
            ))}
         </div>
      </div>
   );
}
