import { makeStyles } from '@material-ui/core';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { RefObject, useEffect, useState } from 'react';
import useThrottledResizeObserver from 'src/hooks/useThrottledResizeObserver';
import useConsumerStatusInfo from 'src/store/webrtc/hooks/useConsumerStatusInfo';

const useStyles = makeStyles({
   root: {
      backgroundColor: 'rgba(0,0,0,0.4)',
      fontSize: 11,
      padding: 4,
   },
});

type Props = {
   consumer: Consumer;
   videoElement: RefObject<any> | null | undefined;
};

export default function ConsumerDiagnosticInfo({ consumer, videoElement }: Props) {
   const classes = useStyles();

   const [videoWidth, setVideoWidth] = useState<number | undefined>();
   const [videoHeight, setVideoHeight] = useState<number | undefined>();
   const info = useConsumerStatusInfo(consumer.id);

   const [tileSize] = useThrottledResizeObserver(100, { ref: videoElement });

   useEffect(() => {
      const updateVideoSize = () => {
         const settings = consumer.track.getSettings();
         setVideoWidth(settings.width);
         setVideoHeight(settings.height);
      };

      const token = setInterval(updateVideoSize, 500);
      return () => clearInterval(token);
   }, []);

   return (
      <div className={classes.root}>
         <span>Scalability Mode: {consumer.rtpParameters.encodings?.[0].scalabilityMode}</span>
         <br />
         <span>Codec: {consumer.rtpParameters.codecs[0].mimeType.split('/')[1]}</span>
         <br />
         <span>Score: {info?.score?.score}</span>
         <br />
         <span>Producer Score: {info?.score?.producerScore}</span>
         <br />
         <span>Current Spatial Layers: {info?.currentLayers?.spatialLayer}</span>
         <br />
         <span>Current Temporal Layers: {info?.currentLayers?.temporalLayer}</span>
         <br />
         <span>Preferred Spatial Layers: {info?.preferredLayers?.spatialLayer}</span>
         <br />
         <span>Preferred Temporal Layers: {info?.preferredLayers?.temporalLayer}</span>
         <br />
         <span>
            Video Size: {videoWidth}x{videoHeight}
         </span>
         <br />
         <span>
            Tile Size: {tileSize ? Math.round(tileSize.width) : 'unknown'}x
            {tileSize ? Math.round(tileSize.height) : 'unknown'}
         </span>
      </div>
   );
}
