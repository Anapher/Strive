import { makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import ErrorWrapper from 'src/components/ErrorWrapper';
import RenderConsumerVideo from 'src/components/RenderConsumerVideo';
import useDeviceManagement from 'src/features/media/useDeviceManagement';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
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
   const myId = useMyParticipantId();
   const { t } = useTranslation();
   const consumer = useConsumer(myId, 'loopback-webcam');

   const localCam = useWebcam(true);
   const camDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);

   const camController = useDeviceManagement('loopback-webcam', localCam, camDevice);
   const [error, setError] = useState<string | null>(null);

   const handleEnableWebcam = async () => {
      try {
         await camController.enable();
      } catch (error) {
         if (error.toString().startsWith('NotAllowedError')) {
            setError(t('conference.settings.webcam.permission_denied'));
         } else {
            setError(error.toString());
         }
      }
   };

   useEffect(() => {
      // cam mic is automatically disabled on component unmount
      handleEnableWebcam();
   }, []);

   return (
      <div>
         <Typography variant="h6" gutterBottom>
            {t('conference.settings.test')}
         </Typography>
         <ErrorWrapper failed={!!error} error={error} onRetry={handleEnableWebcam}>
            <Typography gutterBottom>{t('conference.settings.webcam.test_description')}</Typography>
            <div className={classes.videoContainer}>
               <RenderConsumerVideo consumer={consumer} height={360} width={640} />
            </div>
         </ErrorWrapper>
      </div>
   );
}
