import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import { motion } from 'framer-motion';
import React from 'react';
import { Participant } from 'src/features/conference/types';
import useSomeParticipants from '../useSomeParticipants';
import ParticipantTile from './ParticipantTile';

const useStyles = makeStyles({
   rootBottom: {
      position: 'absolute',
      bottom: 20,
      right: 12,
      left: 12,
      display: 'flex',
      flexDirection: 'row-reverse',
   },
   rootRight: {
      position: 'absolute',
      bottom: 12,
      right: 12,
      top: 12,
      display: 'flex',
      flexDirection: 'column',
   },
});

type Props = {
   location: 'bottom' | 'right';
   tileHeight: number;
   tileWidth: number;

   fixedParticipants?: Participant[];
};

export default function PresentationSceneParticipants({ location, tileWidth, tileHeight, fixedParticipants }: Props) {
   const classes = useStyles();
   const participants = useSomeParticipants(4, fixedParticipants).reverse();

   return (
      <div className={clsx({ [classes.rootBottom]: location === 'bottom', [classes.rootRight]: location === 'right' })}>
         {participants.map((x, i) => (
            <motion.div
               initial={{ scale: 0 }}
               animate={{ scale: 1 }}
               key={x.id}
               style={{ width: tileWidth, height: tileHeight, marginLeft: i === 0 ? 8 : 0 }}
            >
               <ParticipantTile participant={x} width={tileWidth} height={tileHeight} disableLayoutAnimation />
            </motion.div>
         ))}
      </div>
   );
}
