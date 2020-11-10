import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import _ from 'lodash';
import React from 'react';
import { useSelector } from 'react-redux';
import { ParticipantDto } from 'src/features/conference/types';
import { RootState } from 'src/store';
import { Size } from 'src/types';
import { expandToBox, generateGrid, maxWidth } from '../calculations';
import ParticipantTile from './ParticipantTile';

const useStyles = makeStyles((theme) => ({
   center: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
   },
   tilesGrid: {
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
      justifyContent: 'space-between',
   },
   tilesRow: {
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'space-between',
   },
   flex: {
      flex: 1,
   },
}));

type Props = {
   className?: string;
   dimensions: Size;
};

export default function ParticipantsGrid({ dimensions, className }: Props) {
   let participants = useSelector((state: RootState) => state.conference.participants);
   const classes = useStyles();

   if (!participants) return <div className={className} />;

   participants = Array.from({ length: 32 }).map((_, i) => ({
      displayName: 'test',
      participantId: i.toString(),
      attributes: {},
      role: 'mod',
   }));

   if (!dimensions || !dimensions.width || !dimensions.height) return <div />;

   const spacing = 8;
   const grid = generateGrid(participants.length, 640, dimensions, spacing);
   console.log(grid);

   return (
      <div className={clsx(className, classes.center)}>
         <div className={classes.tilesGrid} style={{ width: grid.containerWidth, height: grid.containerHeight }}>
            {Array.from({ length: grid.rows }).map((foo, i) => (
               <div key={i.toString()} className={classes.tilesRow}>
                  {_(participants)
                     .drop(i * grid.itemsPerRow)
                     .take(grid.itemsPerRow)
                     .value()
                     .map((x, pi) => (
                        <div
                           key={x.participantId}
                           style={{ ...grid.itemSize, backgroundColor: 'red', marginLeft: pi === 0 ? 0 : spacing }}
                        >
                           <ParticipantTile />
                        </div>
                     ))}
               </div>
            ))}
         </div>
      </div>
   );
}
