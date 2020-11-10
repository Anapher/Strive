import { makeStyles } from '@material-ui/core';
import React, { useRef } from 'react';
import MediaControls from './MediaControls';
import ParticipantsGrid from './ParticipantsGrid';

const useStyles = makeStyles({
   root: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
   },
   videoContainer: {
      flex: 1,
      position: 'relative',
      paddingTop: '56.25%',
   },
   video: {
      position: 'absolute',
      top: 0,
      left: 0,
      width: '100%',
      height: '100%',
   },
});

export default function Media() {
   const videoElem = useRef<HTMLVideoElement>(null);
   const classes = useStyles();

   return (
      <div className={classes.root}>
         {/* <div className={classes.videoContainer}>
            <video autoPlay ref={videoElem} style={{ backgroundColor: 'black' }} className={classes.video} />
         </div> */}
         <div>
            <MediaControls />
         </div>
      </div>
   );
}
