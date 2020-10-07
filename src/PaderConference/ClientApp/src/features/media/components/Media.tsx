import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { getRtc, RootState } from 'src/store';
import { send } from 'src/store/conference-signal/actions';
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
         const rtc = getRtc();
         rtc.createConnection();
      }
   }, [connected]);

   const startStream = async () => {
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia({ video: true })) as MediaStream;

      videoElem.current!.srcObject = stream;

      stream.getTracks().forEach((track) => getRtc().getConnection()?.addTrack(track, stream));
   };

   const getScreen = async () => {
      const conn = getRtc().getConnection();
      if (conn) {
         conn.ontrack = (ev) => (videoElem.current!.srcObject = ev.streams[0]);

         dispatch(send('RequestVideo'));
      }
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
            <MediaControls startDesktopRecording={startStream} getScreen={getScreen} />
         </div>
      </div>
   );
}
