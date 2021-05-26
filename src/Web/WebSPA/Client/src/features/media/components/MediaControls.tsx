import { Dialog, DialogContent, DialogTitle, Fab, Grid, makeStyles, Tooltip } from '@material-ui/core';
import BugReportIcon from '@material-ui/icons/BugReport';
import clsx from 'classnames';
import { motion } from 'framer-motion';
// import { HumanHandsup } from 'mdi-material-ui';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import AnimatedCamIcon from 'src/assets/animated-icons/AnimatedCamIcon';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import AnimatedScreenIcon from 'src/assets/animated-icons/AnimatedScreenIcon';
import Debug from 'src/features/conference/components/troubleshoot/Troubleshooting';
import { selectIsDeviceAvailable } from 'src/features/settings/selectors';
import usePermission from 'src/hooks/usePermission';
import {
   // CONFERENCE_CAN_RAISE_HAND,
   MEDIA_CAN_SHARE_AUDIO,
   MEDIA_CAN_SHARE_SCREEN,
   MEDIA_CAN_SHARE_WEBCAM,
} from 'src/permissions';
import { RootState } from 'src/store';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import useScreen from 'src/store/webrtc/hooks/useScreen';
import useWebcam from 'src/store/webrtc/hooks/useWebcam';
import useDeviceManagement from '../useDeviceManagement';
import MediaFab from './MediaFab';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'row',
      backgroundImage: 'linear-gradient(to bottom, rgba(6, 6, 7, 0), rgba(6, 6, 7, 0.7), rgba(6, 6, 7, 1))',
      paddingBottom: 16,
      padding: theme.spacing(0, 2, 1),
   },
   leftActions: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row',
   },
   rightActions: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row-reverse',
   },
   fab: {
      margin: theme.spacing(0, 1),
   },
   dialog: {
      backgroundColor: theme.palette.background.default,
   },
   controlsContainer: {
      display: 'flex',
      flexDirection: 'row',
   },
}));

type Props = {
   className?: string;
   show: boolean;
   leftActionsRef: React.Ref<HTMLDivElement>;
};

const variants = {
   visible: {
      opacity: 1,
      transition: {
         staggerChildren: 0.1,
      },
   },
   hidden: {
      opacity: 0,
   },
};

const item = {
   visible: { opacity: 1, scale: 1 },
   hidden: { opacity: 0, scale: 0 },
};

export default function MediaControls({ className, show, leftActionsRef }: Props) {
   const classes = useStyles();
   const { t } = useTranslation();

   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('mic', localMic, audioDevice);
   const micAvailable = useSelector((state: RootState) => selectIsDeviceAvailable(state, 'mic'));

   const webcamDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);
   const localWebcam = useWebcam();
   const webcamController = useDeviceManagement('webcam', localWebcam, webcamDevice);
   const webcamAvailable = useSelector((state: RootState) => selectIsDeviceAvailable(state, 'webcam'));

   const screenDevice = useSelector((state: RootState) => state.settings.obj.screen.device);
   const localScreen = useScreen();
   const screenController = useDeviceManagement('screen', localScreen, screenDevice);
   const screenAvailable = useSelector((state: RootState) => selectIsDeviceAvailable(state, 'screen'));

   const canShareScreen = usePermission(MEDIA_CAN_SHARE_SCREEN);
   const canShareAudio = usePermission(MEDIA_CAN_SHARE_AUDIO);
   const canShareWebcam = usePermission(MEDIA_CAN_SHARE_WEBCAM);

   const [debugDialogOpen, setDebugDialogOpen] = useState(false);

   const handleCloseDebugDialog = () => setDebugDialogOpen(false);
   const handleOpenDebugDialog = () => setDebugDialogOpen(true);

   return (
      <motion.div
         className={clsx(classes.root, className)}
         initial="hidden"
         animate={show ? 'visible' : 'hidden'}
         variants={variants}
      >
         <Grid container spacing={1} className={classes.leftActions} ref={leftActionsRef} />
         <div className={classes.controlsContainer}>
            {canShareScreen && screenAvailable && (
               <MediaFab
                  translationKey="screen"
                  className={classes.fab}
                  Icon={AnimatedScreenIcon}
                  control={screenController}
                  component={motion.button}
                  variants={item}
               />
            )}
            {canShareWebcam && (
               <MediaFab
                  disabled={!webcamAvailable}
                  translationKey="webcam"
                  className={classes.fab}
                  Icon={AnimatedCamIcon}
                  control={webcamController}
                  component={motion.button}
                  variants={item}
               />
            )}
            {canShareAudio && (
               <MediaFab
                  disabled={!micAvailable}
                  translationKey="mic"
                  className={classes.fab}
                  Icon={AnimatedMicIcon}
                  control={micController}
                  pauseOnToggle
                  component={motion.button}
                  variants={item}
               />
            )}
         </div>
         <div className={classes.rightActions}>
            <Tooltip title={t<string>('conference.troubleshooting.title')} arrow>
               <Fab
                  color="default"
                  className={classes.fab}
                  onClick={handleOpenDebugDialog}
                  component={motion.button}
                  variants={item}
                  aria-label={t('conference.troubleshooting.title')}
               >
                  <BugReportIcon />
               </Fab>
            </Tooltip>
         </div>
         <Dialog open={debugDialogOpen} onClose={handleCloseDebugDialog} PaperProps={{ className: classes.dialog }}>
            <DialogTitle>{t('conference.troubleshooting.title')}</DialogTitle>
            <DialogContent>
               <Debug />
            </DialogContent>
         </Dialog>
      </motion.div>
   );
}
