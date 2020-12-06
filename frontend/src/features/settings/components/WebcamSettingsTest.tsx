import { makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { selectMyParticipantId } from 'src/features/auth/selectors';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import useWebcam from 'src/store/webrtc/hooks/useWebcam';

const useStyles = makeStyles((theme) => ({
   videoContainer: {
      position: 'relative',
      borderRadius: theme.shape.borderRadius,
      backgroundColor: theme.palette.background.paper,
      boxShadow: theme.shadows[6],

      paddingBottom: '56.25%',
      maxWidth: 640,
   },
   video: {
      position: 'absolute',
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: '100%',
      height: '100%',
      borderRadius: theme.shape.borderRadius,
      objectFit: 'cover',
   },
}));

export default function WebcamSettingsTest() {
   const classes = useStyles();
   const myId = useSelector(selectMyParticipantId);
   const consumer = useConsumer(myId, 'loopback-webcam');

   const localCam = useWebcam(true);
   const camDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);
   console.log('camdevice', camDevice);

   const camController = useDeviceManagement('loopback-webcam', localCam, camDevice);

   useEffect(() => {
      // cam mic is automatically disabled on component unmount
      camController.enable();
   }, []);

   const videoElem = useRef<HTMLVideoElement>(null);

   useEffect(() => {
      if (consumer && videoElem.current) {
         console.log('start video');

         const stream = new MediaStream();
         stream.addTrack(consumer.track);

         videoElem.current.srcObject = stream;
         videoElem.current.play();
      }
   }, [consumer, videoElem.current]);

   return (
      <div>
         <Typography variant="h6" gutterBottom>
            Test
         </Typography>
         <Typography gutterBottom>
            We will loopback your current webcam input over the server and show the results here. This way, you can
            check what other participants actually receive and you also get a taste of the actual delay.
         </Typography>
         <div className={classes.videoContainer}>
            <video ref={videoElem} className={classes.video} />
         </div>
      </div>
   );
}
