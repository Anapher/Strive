import { Fab, makeStyles } from '@material-ui/core';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import React from 'react';
import usePermission, {
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
import { useMicrophone } from 'src/store/webrtc/useMicrophone';
import { RootState } from 'src/store';
import MediaFab from './MediaFab';
import useSoupManager from 'src/store/webrtc/useSoupManager';

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'row',
   },
   leftActions: {
      flex: 1,
      display: 'flex',
      flexDirection: 'row-reverse',
   },
   fab: {
      margin: theme.spacing(0, 1),
   },
}));

export default function MediaControls() {
   const classes = useStyles();

   const soupManager = useSoupManager();

   const micState = useMicrophone(soupManager);

   const canShareScreen = usePermission(MEDIA_CAN_SHARE_SCREEN);
   const canShareAudio = usePermission(MEDIA_CAN_SHARE_AUDIO);
   const canShareWebcam = usePermission(MEDIA_CAN_SHARE_WEBCAM);

   return (
      <div className={classes.root}>
         <div className={classes.leftActions}></div>
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
         <div className={classes.leftActions}>
            <Fab color="default" aria-label="share microphone" className={classes.fab}>
               <BugReportIcon />
            </Fab>
         </div>
      </div>
   );
}
