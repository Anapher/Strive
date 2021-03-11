import { IconButton, makeStyles, Paper, Typography } from '@material-ui/core';
import React from 'react';
import { ChatMessageDto } from 'src/core-hub.types';
import { Bullhorn } from 'mdi-material-ui';
import { motion, MotionProps } from 'framer-motion';
import clsx from 'classnames';
import CloseIcon from '@material-ui/icons/Close';

const useStyles = makeStyles((theme) => ({
   root: {
      minWidth: '50vw',
      display: 'flex',
      flexDirection: 'row',
      alignItems: 'flex-start',
      backgroundColor: theme.palette.background.paper,

      [theme.breakpoints.up('sm')]: {
         maxWidth: '80vw',
      },
   },
   header: {
      display: 'flex',
      alignItems: 'flex-end',
   },
   headerIcon: {
      color: theme.palette.text.secondary,
      marginBottom: 4,
      marginLeft: theme.spacing(1),
      width: 18,
   },
   content: {
      padding: theme.spacing(2),
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
   },
}));

type Props = {
   message: ChatMessageDto;
   className?: string;
   onClose: () => void;
};

export default function AnnouncementCard({ message, className, onClose }: Props) {
   const classes = useStyles();

   const paperProps: MotionProps = {
      initial: { scale: 2, opacity: 0 },
      animate: { scale: 1, opacity: 1 },
      exit: { opacity: 0 },
   };

   return (
      <Paper
         className={clsx(classes.root, className)}
         component={motion.div as any}
         {...(paperProps as any)}
         elevation={5}
      >
         <div className={classes.content}>
            <div className={classes.header}>
               <Typography color="textSecondary">{message.sender?.meta.displayName}</Typography>
               <Bullhorn className={classes.headerIcon} />
            </div>

            <Typography align="center" variant="h6">
               {message.message}
            </Typography>
         </div>
         <IconButton onClick={onClose}>
            <CloseIcon />
         </IconButton>
      </Paper>
   );
}
