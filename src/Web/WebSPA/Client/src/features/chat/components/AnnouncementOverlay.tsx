import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { ChatMessageDto } from 'src/core-hub.types';
import { selectAnnouncements } from '../selectors';
import AnnouncementCard from './AnnouncementCard';
import { removeAnnouncement } from '../reducer';
import { AnimatePresence } from 'framer-motion';

const useStyles = makeStyles((theme) => ({
   root: {
      position: 'absolute',
      zIndex: theme.zIndex.modal,
      marginLeft: 'auto',
      marginRight: 'auto',
      left: 0,
      right: 0,
      top: theme.spacing(2),
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
   },
   notFirstCard: {
      marginTop: theme.spacing(2),
   },
}));

export default function AnnouncementOverlay() {
   const classes = useStyles();
   const announcements = useSelector(selectAnnouncements);
   const dispatch = useDispatch();

   const handleCloseCard = (message: ChatMessageDto) => {
      dispatch(removeAnnouncement(message));
   };

   return (
      <div className={classes.root}>
         <AnimatePresence>
            {announcements.map((message, i) => (
               <AnnouncementCard
                  key={message.id}
                  message={message}
                  className={i === 0 ? undefined : classes.notFirstCard}
                  onClose={() => handleCloseCard(message)}
               />
            ))}
         </AnimatePresence>
      </div>
   );
}
