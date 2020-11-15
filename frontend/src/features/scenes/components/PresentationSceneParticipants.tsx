import { makeStyles } from '@material-ui/core';
import React from 'react';
import clsx from 'classnames';
import { motion } from 'framer-motion';

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

   return (
      <div className={clsx({ [classes.rootBottom]: location === 'bottom', [classes.rootRight]: location === 'right' })}>
         {Array.from({ length: 4 }).map((_, i) => (
            <motion.div
               layout
               layoutId={'p-' + i}
               key={i}
               style={{
                  width: tileWidth,
                  height: tileHeight,
                  backgroundColor: 'red',
                  margin: 4,
               }}
            />
         ))}
      </div>
   );
}
