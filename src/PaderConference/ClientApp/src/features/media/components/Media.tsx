import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { initialize } from 'src/store/webrtc/actions';
import MediaControls from './MediaControls';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100%',
   },
});

export default function Media() {
   const videoElem = useRef<HTMLVideoElement>(null);
   const classes = useStyles();
   const dispatch = useDispatch();

   const connected = useSelector((state: RootState) => state.signalr.isConnected);

   useEffect(() => {
      if (connected) {
         dispatch(initialize());
      }
   }, [connected]);

   return (
      <div className={classes.root}>
         <div>
            <video
               width={1280}
               height={720}
               autoPlay
               ref={videoElem}
               style={{ backgroundColor: 'black', marginBottom: 32 }}
            />
            <MediaControls />
         </div>
      </div>
   );
}
