import { makeStyles } from '@material-ui/core';
import { motion } from 'framer-motion';
import _ from 'lodash';
import { Participant } from 'src/features/conference/types';
import { generateGrid } from '../../calculations';
import ParticipantTile from '../ParticipantTile';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100%',
      width: '100%',
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
});

type Props = {
   participants: Participant[];
   spacing?: number;

   width: number;
   height: number;

   itemMaxWidth?: number;
};

export default function RenderGrid({ participants, spacing = 8, itemMaxWidth = 640, width, height }: Props) {
   const classes = useStyles();
   if (participants.length === 0) return null;

   const grid = generateGrid(participants.length, itemMaxWidth, { width, height }, spacing);

   return (
      <div className={classes.root}>
         <div className={classes.tilesGrid} style={{ width: grid.containerWidth, height: grid.containerHeight }}>
            {Array.from({ length: grid.rows }).map((__, i) => (
               <div key={i.toString()} className={classes.tilesRow}>
                  {_(participants)
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
