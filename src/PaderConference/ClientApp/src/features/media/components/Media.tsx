import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
import { getMediasoup, RootState } from 'src/store';
import { send } from 'src/store/conference-signal/actions';
import { initialize } from 'src/store/webrtc/actions';
import useConsumers from 'src/store/webrtc/useConsumers';
import { useScreen } from 'src/store/webrtc/useScreen';
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
         dispatch(initialize());
      }
   }, [connected]);

   useEffect(() => {
      if (mediaInfo?.isScreenshareActivated && accessToken?.nameid !== mediaInfo.partipantScreensharing) {
         dispatch(send('RequestVideo'));
      }
   }, [mediaInfo?.isScreenshareActivated]);

   const { enable, stream } = useScreen(getMediasoup());
   const consumers = useConsumers(getMediasoup());

   useEffect(() => {
      if (consumers.length > 0) {
         console.log(consumers);

         const stream = new MediaStream();
         stream.addTrack(consumers[0].track);

         videoElem.current!.srcObject = stream;
      }
   }, [consumers]);

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
            <MediaControls startDesktopRecording={enable} />
         </div>
      </div>
   );
}
