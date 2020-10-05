import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { getRtc, RootState } from 'src/store';
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

   const connected = useSelector((state: RootState) => state.signalr.isConnected);

   useEffect(() => {
      if (connected) {
         const rtc = getRtc();
         rtc.createConnection();
      }
   }, [connected]);

   const startStream = async () => {
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia({ video: true })) as MediaStream;
      console.log(stream);

      videoElem.current!.srcObject = stream;

      stream.getTracks().forEach((track) => getRtc().getConnection()?.addTrack(track, stream));
   };

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
            <MediaControls startDesktopRecording={startStream} />
         </div>
      </div>
   );
}
