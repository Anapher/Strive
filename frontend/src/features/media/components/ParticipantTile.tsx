import { makeStyles } from '@material-ui/core';
import { motion } from 'framer-motion';
import React from 'react';

const useStyles = makeStyles((theme) => ({
   content: {
      borderRadius: 8,
      backgroundColor: theme.palette.background.paper,
      borderWidth: 1,
      boxShadow: theme.shadows[6],
   },
   border: {
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      borderRadius: 8,
      borderWidth: 2,
      border: 'solid red',
   },
}));

type Props = {
   className?: string;
};

export default function ParticipantTile({ className }: Props) {
   const classes = useStyles();

   return (
      <motion.div whileHover={{ scale: 1.05, zIndex: 500 }} className={className}>
         <motion.div className={classes.content}></motion.div>
         {/* <motion.div
            className={classes.border}
            animate={{ scale: [1, 1.05, 1], opacity: [1, 0, 0] }}
            transition={{
               duration: 4,
               ease: 'easeInOut',
               loop: Infinity,
            }}
         ></motion.div> */}
      </motion.div>
   );
}
