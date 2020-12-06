import { Dialog, DialogContent, DialogTitle, Fab, makeStyles, Tooltip } from '@material-ui/core';
import BugReportIcon from '@material-ui/icons/BugReport';
import clsx from 'classnames';
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
};

export default function MediaControls({ className }: Props) {
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
      <div className={clsx(classes.root, className)}>
         <div className={classes.leftActions}>
            {canRaiseHand && (
               <Tooltip title="Raise Hand" aria-label="raise hand">
                  <Fab color="secondary" className={classes.fab}>
                     <HumanHandsup />
                  </Fab>
               </Tooltip>
            )}
         </div>
         <div style={{ display: 'flex', flexDirection: 'row' }}>
            {canShareScreen && (
               <MediaFab
                  aria-label="share screen"
                  className={classes.fab}
                  Icon={AnimatedScreenIcon}
                  control={screenController}
               />
            )}
            {canShareWebcam && (
               <MediaFab
                  aria-label="share webcam"
                  className={classes.fab}
                  Icon={AnimatedCamIcon}
                  control={webcamController}
               />
            )}
            {canShareAudio && (
               <MediaFab
                  aria-label="share microphone"
                  className={classes.fab}
                  Icon={AnimatedMicIcon}
                  control={micController}
                  pauseOnToggle
               />
            )}
         </div>
         <div className={classes.rightActions}>
            <Tooltip title="Troubleshooting" aria-label="troubleshooting">
               <Fab color="default" className={classes.fab} onClick={handleOpenDebugDialog}>
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
      </div>
   );
}
