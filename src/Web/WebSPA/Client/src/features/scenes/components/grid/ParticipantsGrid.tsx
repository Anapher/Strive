import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { motion } from 'framer-motion';
import _ from 'lodash';
import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import { generateGrid } from '../../calculations';
import { GridScene, RenderSceneProps } from '../../types';
import ParticipantTile from '../ParticipantTile';

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

export default function ParticipantsGrid({
   dimensions,
   className,
   setShowWebcamUnderChat,
   next,
}: RenderSceneProps<GridScene>) {
   const participants = useSelector(selectParticipants);
   const participantsOfRoom = useSelector(selectParticipantsOfCurrentRoom)
      .map((id) => participants[id])
      .filter((x): x is Participant => Boolean(x));

   const classes = useStyles();

   useEffect(() => {
      setShowWebcamUnderChat(false);
   }, [setShowWebcamUnderChat]);

   const overwrite = next();
   if (overwrite) return <>{overwrite}</>;

   if (!participantsOfRoom || participantsOfRoom.length === 0) return null;

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
                           layoutId={x.id}
                           key={x.id}
                           style={{ ...grid.itemSize, marginLeft: pi === 0 ? 0 : spacing }}
                        >
                           <ParticipantTile participant={x} {...grid.itemSize} />
                        </motion.div>
                     ))}
               </div>
            ))}
         </div>
      </div>
   );
}
