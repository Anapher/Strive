import { Button, makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
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
   const mediaInfo = useSelector((state: RootState) => state.media.synchronized);
   const accessToken = useSelector(selectAccessToken);

   useEffect(() => {
      if (connected) {
         const rtc = getRtc();
         rtc.createConnection();

         const conn = rtc.getConnection()!;
         conn.ontrack = (e) => {
            console.log('track event muted = ' + e.track.muted);
            e.track.onunmute = () => {
               console.log('track unmuted');
               console.log(e);

               videoElem.current!.srcObject = new MediaStream([e.track]);
            };
         };
      }
   }, [connected]);

   useEffect(() => {
      if (mediaInfo?.isScreenshareActivated && accessToken?.nameid !== mediaInfo.partipantScreensharing) {
         dispatch(send('RequestVideo'));
      }
   }, [mediaInfo?.isScreenshareActivated]);

   const startStream = async () => {
      const constraints: MediaStreamConstraints = { video: { height: { ideal: 720 }, frameRate: 25 } };
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia(constraints)) as MediaStream;

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
