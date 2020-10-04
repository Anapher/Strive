import { makeStyles } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
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

   const pcRef = useRef<RTCPeerConnection | null>(null);

   useEffect(() => {
      pcRef.current = new RTCPeerConnection();

      const pc = pcRef.current;
      pc.onicecandidate = ({ candidate }) => console.log(candidate);
      pc.onnegotiationneeded = async () => {
         try {
            await pc.setLocalDescription(await pc.createOffer());
            // send the offer to the other peer
            console.log({ desc: pc.localDescription });
         } catch (err) {
            console.error(err);
         }
      };

      return pc.close;
   }, []);

   const startStream = async () => {
      const stream = (await (navigator.mediaDevices as any).getDisplayMedia({ video: true })) as MediaStream;
      console.log(stream);

      videoElem.current!.srcObject = stream;

      stream.getTracks().forEach((track) => pc.current!.addTrack(track, stream));
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
