import { makeStyles } from '@material-ui/core';
import { Consumer } from 'mediasoup-client/lib/Consumer';
import React, { useEffect, useState } from 'react';
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
   tileWidth: number;
   tileHeight: number;
};

export default function ConsumerDiagnosticInfo({ consumer, tileWidth, tileHeight }: Props) {
   const classes = useStyles();

   const [videoWidth, setVideoWidth] = useState<number | undefined>();
   const [videoHeight, setVideoHeight] = useState<number | undefined>();
   const info = useConsumerStatusInfo(consumer.id);

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
         <span>Prefferred Spatial Layers: {info?.prefferredLayers?.spatialLayer}</span>
         <br />
         <span>Prefferred Temporal Layers: {info?.prefferredLayers?.temporalLayer}</span>
         <br />
         <span>
            Video Size: {videoWidth}x{videoHeight}
         </span>
         <br />
         <span>
            Tile Size: {Math.round(tileWidth)}x{Math.round(tileHeight)}
         </span>
      </div>
   );
}
