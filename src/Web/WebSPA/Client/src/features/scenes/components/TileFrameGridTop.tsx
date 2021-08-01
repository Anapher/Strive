import { makeStyles } from '@material-ui/core';
import React from 'react';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import { GridTopInstructions } from '../tile-frame-calculations';
import ParticipantTile from './ParticipantTile';

const useStyles = makeStyles({
   container: {
      width: '100%',
      height: '100%',
      display: 'flex',
      justifyContent: 'flex-start',
      flexDirection: 'column',
   },
   participantTilesContainer: {
      display: 'flex',
      width: '100%',
      flexDirection: 'column',
      alignItems: 'center',
   },
   participantTilesRow: {
      display: 'flex',
      flexDirection: 'row',
   },
});

type Props = {
   instructions: GridTopInstructions;
   participants: Participant[];
   render: (size: Size) => React.ReactNode;
};

export default function TileFrameGridTop({ instructions, participants, render }: Props) {
   const classes = useStyles();

   return (
      <div className={classes.container}>
         <div
            className={classes.participantTilesContainer}
            style={{
               paddingLeft: instructions.tilesMargin.left,
               paddingRight: instructions.tilesMargin.right,
               paddingTop: instructions.tilesMargin.top,
               paddingBottom: instructions.tilesMargin.bottom,
            }}
         >
            <div className={classes.participantTilesRow}>
               {participants.slice(0, instructions.tileAmount).map((x, i) => (
                  <div
                     key={x.id}
                     style={{ ...instructions.tileSize, marginLeft: i === 0 ? 0 : instructions.tileSpaceBetween }}
                  >
                     <ParticipantTile {...instructions.tileSize} participant={x} />
                  </div>
               ))}
            </div>
         </div>
         <div
            style={{
               marginLeft: instructions.contentMargin.left,
               marginRight: instructions.contentMargin.right,
               marginTop: instructions.contentMargin.top,
               marginBottom: instructions.contentMargin.bottom,
               flex: 1,
            }}
         >
            {render(instructions.contentDimensions)}
         </div>
      </div>
   );
}
