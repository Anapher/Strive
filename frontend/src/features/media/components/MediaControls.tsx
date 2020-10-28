import { Dialog, DialogContent, DialogTitle, Fab, makeStyles, Tooltip } from '@material-ui/core';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import React, { useState } from 'react';
import usePermission, {
   CONFERENCE_CAN_RAISE_HAND,
   MEDIA_CAN_SHARE_AUDIO,
   MEDIA_CAN_SHARE_SCREEN,
   MEDIA_CAN_SHARE_WEBCAM,
} from 'src/hooks/usePermission';
import VideocamIcon from '@material-ui/icons/Videocam';
import MicIcon from '@material-ui/icons/Mic';
import MicOffIcon from '@material-ui/icons/MicOff';
import BugReportIcon from '@material-ui/icons/BugReport';
import DesktopAccessDisabledIcon from '@material-ui/icons/DesktopAccessDisabled';
import VideocamOffIcon from '@material-ui/icons/VideocamOff';
import { useMicrophone } from 'src/store/webrtc/hooks/useMicrophone';
import MediaFab from './MediaFab';
import PanToolIcon from '@material-ui/icons/PanTool';
import Debug from 'src/features/conference/components/Troubleshooting';
import { RootState } from 'src/store';
import { useSelector } from 'react-redux';

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

export default function MediaControls() {
   const classes = useStyles();

   const gain = useSelector((state: RootState) => state.settings.obj.audioGain);
   const micState = useMicrophone(gain);

   const canShareScreen = usePermission(MEDIA_CAN_SHARE_SCREEN);
   const canShareAudio = usePermission(MEDIA_CAN_SHARE_AUDIO);
   const canShareWebcam = usePermission(MEDIA_CAN_SHARE_WEBCAM);
   const canRaiseHand = usePermission(CONFERENCE_CAN_RAISE_HAND);

   const [debugDialogOpen, setDebugDialogOpen] = useState(false);

   const handleCloseDebugDialog = () => setDebugDialogOpen(false);
   const handleOpenDebugDialog = () => setDebugDialogOpen(true);

   return (
      <div className={classes.root}>
         <div className={classes.leftActions}>
            {canRaiseHand && (
               <Tooltip title="Raise Hand" aria-label="raise hand">
                  <Fab color="secondary" className={classes.fab}>
                     <PanToolIcon />
                  </Fab>
               </Tooltip>
            )}
         </div>
         <div style={{ display: 'flex', flexDirection: 'row' }}>
            {canShareScreen && (
               <Fab color="primary" aria-label="share screen" className={classes.fab}>
                  <DesktopAccessDisabledIcon />
               </Fab>
            )}
            {canShareWebcam && (
               <Fab color="primary" aria-label="share webcam" className={classes.fab}>
                  <VideocamOffIcon />
               </Fab>
            )}
            {canShareAudio && (
               <MediaFab
                  aria-label="share microphone"
                  className={classes.fab}
                  IconEnable={MicIcon}
                  IconDisable={MicOffIcon}
                  mediaState={micState}
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
