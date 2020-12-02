import { makeStyles } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { motion } from 'framer-motion';
import { useSelector } from 'react-redux';
import { selectSceneOverlayParticipants } from '../selectors';
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
};

export default function PresentationSceneParticipants({ location, tileWidth, tileHeight }: Props) {
   const classes = useStyles();
   const sceneParticipants = useSelector(selectSceneOverlayParticipants);
   const participants = [...sceneParticipants];

   const tileSize = { width: tileWidth, height: tileHeight };

   return (
      <div className={clsx({ [classes.rootBottom]: location === 'bottom', [classes.rootRight]: location === 'right' })}>
         {participants.map((x) => (
            <motion.div
               initial={{ scale: 0 }}
               animate={{ scale: 1 }}
               key={x.participantId}
               style={{ ...tileSize, marginLeft: 0 }}
            >
               <ParticipantTile participant={x} size={tileSize} disableLayoutAnimation />
            </motion.div>
         ))}
      </div>
   );
}
