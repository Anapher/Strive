import { Dialog, DialogContent, DialogTitle, Fab, makeStyles, Tooltip } from '@material-ui/core';
import BugReportIcon from '@material-ui/icons/BugReport';
import clsx from 'classnames';
import { motion } from 'framer-motion';
import { HumanHandsup } from 'mdi-material-ui';
import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import AnimatedCamIcon from 'src/assets/animated-icons/AnimatedCamIcon';
import AnimatedMicIcon from 'src/assets/animated-icons/AnimatedMicIcon';
import AnimatedScreenIcon from 'src/assets/animated-icons/AnimatedScreenIcon';
import Debug from 'src/features/conference/components/Troubleshooting';
import usePermission, {
   CONFERENCE_CAN_RAISE_HAND,
   MEDIA_CAN_SHARE_AUDIO,
   MEDIA_CAN_SHARE_SCREEN,
   MEDIA_CAN_SHARE_WEBCAM,
} from 'src/hooks/usePermission';
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
}));

type Props = {
   className?: string;
   show: boolean;
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

export default function MediaControls({ className, show }: Props) {
   const classes = useStyles();

   const gain = useSelector((state: RootState) => state.settings.obj.mic.audioGain);

   const localMic = useMicrophone(gain);
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device);
   const micController = useDeviceManagement('mic', localMic, audioDevice);

   const webcamDevice = useSelector((state: RootState) => state.settings.obj.webcam.device);
   const localWebcam = useWebcam();
   const webcamController = useDeviceManagement('webcam', localWebcam, webcamDevice);

   const screenDevice = useSelector((state: RootState) => state.settings.obj.screen.device);
   const localScreen = useScreen();
   const screenController = useDeviceManagement('screen', localScreen, screenDevice);

   const canShareScreen = usePermission(MEDIA_CAN_SHARE_SCREEN);
   const canShareAudio = usePermission(MEDIA_CAN_SHARE_AUDIO);
   const canShareWebcam = usePermission(MEDIA_CAN_SHARE_WEBCAM);
   const canRaiseHand = usePermission(CONFERENCE_CAN_RAISE_HAND);

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
         <div className={classes.leftActions}>
            {canRaiseHand && (
               <Tooltip title="Raise Hand" aria-label="raise hand" arrow>
                  <Fab color="secondary" className={classes.fab} component={motion.button} variants={item}>
                     <HumanHandsup />
                  </Fab>
               </Tooltip>
            )}
         </div>
         <div style={{ display: 'flex', flexDirection: 'row' }}>
            {canShareScreen && (
               <MediaFab
                  title="Screen"
                  className={classes.fab}
                  Icon={AnimatedScreenIcon}
                  control={screenController}
                  component={motion.button}
                  variants={item}
               />
            )}
            {canShareWebcam && (
               <MediaFab
                  title="Webcam"
                  className={classes.fab}
                  Icon={AnimatedCamIcon}
                  control={webcamController}
                  component={motion.button}
                  variants={item}
               />
            )}
            {canShareAudio && (
               <MediaFab
                  title="Microphone"
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
            <Tooltip title="Troubleshooting" aria-label="troubleshooting" arrow>
               <Fab
                  color="default"
                  className={classes.fab}
                  onClick={handleOpenDebugDialog}
                  component={motion.button}
                  variants={item}
               >
                  <BugReportIcon />
               </Fab>
            </Tooltip>
         </div>
         <Dialog open={debugDialogOpen} onClose={handleCloseDebugDialog} PaperProps={{ className: classes.dialog }}>
            <DialogTitle>Troubleshooting</DialogTitle>
            <DialogContent>
               <Debug />
            </DialogContent>
         </Dialog>
      </motion.div>
   );
}
