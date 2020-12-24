import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { motion } from 'framer-motion';
import _ from 'lodash';
import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { ParticipantDto } from 'src/features/conference/types';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import { RootState } from 'src/store';
import { Size } from 'src/types';
import { generateGrid } from '../calculations';
import { GridScene } from '../types';
import ParticipantTile from './ParticipantTile';

const useStyles = makeStyles(() => ({
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
}));

type Props = {
   className?: string;
   dimensions: Size;
   options: GridScene;
   setShowWebcamUnderChat: (show: boolean) => void;
   setAutoHideControls: (autoHide: boolean) => void;
};

export default function ParticipantsGrid({
   dimensions,
   className,
   setShowWebcamUnderChat,
   setAutoHideControls,
}: Props) {
   const participants = useSelector((state: RootState) => state.conference.participants);
   const participantsOfRoom = useSelector(selectParticipantsOfCurrentRoom)
      .map((id) => participants?.find((x) => x.participantId === id))
      .filter((x): x is ParticipantDto => Boolean(x));

   const classes = useStyles();

   useEffect(() => {
      setShowWebcamUnderChat(false);
      setAutoHideControls(false);
   }, []);

   if (!participantsOfRoom) return <div className={className} />;

   const spacing = 8;
   const grid = generateGrid(participantsOfRoom.length, 640, dimensions, spacing);

   return (
      <div className={clsx(className, classes.center)}>
         <div className={classes.tilesGrid} style={{ width: grid.containerWidth, height: grid.containerHeight }}>
            {Array.from({ length: grid.rows }).map((__, i) => (
               <div key={i.toString()} className={classes.tilesRow}>
                  {_(participantsOfRoom)
                     .drop(i * grid.itemsPerRow)
                     .take(grid.itemsPerRow)
                     .value()
                     .map((x, pi) => (
                        <motion.div
                           layout
                           layoutId={x.participantId}
                           key={x.participantId}
                           style={{ ...grid.itemSize, marginLeft: pi === 0 ? 0 : spacing }}
                        >
                           <ParticipantTile participant={x} size={grid.itemSize} />
                        </motion.div>
                     ))}
               </div>
            ))}
         </div>
      </div>
   );
}
