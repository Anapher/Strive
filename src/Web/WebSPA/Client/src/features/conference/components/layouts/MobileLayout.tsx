import { makeStyles } from '@material-ui/core';
import React, { useState } from 'react';
import RoomsList from 'src/features/rooms/components/RoomsList';
import MobileChatTab from './MobileChatTab';
import MobileLayoutActions from './MobileLayoutActions';

const useStyles = makeStyles({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      position: 'relative',
   },
   actionsOverlay: {
      position: 'absolute',
      bottom: 0,
      left: 0,
      right: 0,
   },
});

export default function MobileLayout() {
   const classes = useStyles();
   const [value, setValue] = useState(0);
   const overlayActions = value === 0;

   return (
      <div className={classes.root}>
         {value === 1 && <MobileChatTab />}
         {value === 2 && <RoomsList />}
         <div className={overlayActions ? classes.actionsOverlay : undefined}>
            <MobileLayoutActions selectedTab={value} setSelectedTab={setValue} />
         </div>
      </div>
   );
}
