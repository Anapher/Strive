import {
   Box,
   Divider,
   Grid,
   IconButton,
   ListItemIcon,
   makeStyles,
   MenuItem,
   Slider,
   Typography,
} from '@material-ui/core';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import MicIcon from '@material-ui/icons/Mic';
import VideocamIcon from '@material-ui/icons/Videocam';
import VolumeMuteIcon from '@material-ui/icons/VolumeMute';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import _ from 'lodash';
import React, { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { patchParticipantAudio } from 'src/features/media/reducer';
import { selectParticipantAudioInfo } from 'src/features/media/selectors';
import { RootState } from 'src/store';
import { ParticipantDto } from '../types';
import SendIcon from '@material-ui/icons/Send';
import { fetchPermissions } from 'src/core-hub';

const useStyles = makeStyles((theme) => ({
   infoMenuItem: {
      padding: theme.spacing(0, 1),
   },
}));

type Props = {
   participant: ParticipantDto;
   onClose: () => void;
};

const ParticipantContextMenu = React.forwardRef<HTMLElement, Props>(({ participant, onClose }) => {
   const audioInfo = useSelector((state: RootState) => selectParticipantAudioInfo(state, participant.participantId));
   const dispatch = useDispatch();
   const classes = useStyles();

   // const handleChangeMuted = (muted: boolean) => {
   //    dispatch(patchParticipantAudio({ participantId: participant.participantId, data: { muted } }));
   // };

   const handleChangeVolume = useCallback(
      _.throttle((volume: number) => {
         dispatch(patchParticipantAudio({ participantId: participant.participantId, data: { volume } }));
      }, 200),
      [participant],
   );

   const handleShowPermissions = () => {
      dispatch(fetchPermissions(participant.participantId));
      onClose();
   };

   return (
      <>
         <div className={classes.infoMenuItem}>
            <Typography>{participant.displayName}</Typography>
            {audioInfo && (
               <Box display="flex" alignItems="center" style={{ marginTop: 4, marginBottom: 4 }}>
                  <IconButton size="small">
                     {audioInfo.muted ? <VolumeMuteIcon fontSize="small" /> : <VolumeUpIcon fontSize="small" />}
                  </IconButton>
                  <Slider
                     style={{ flex: 1, marginLeft: 8 }}
                     value={audioInfo.muted ? 0 : Math.round(audioInfo.volume * 100)}
                     onChange={(_event: unknown, newValue: number | number[]) =>
                        handleChangeVolume((newValue as number) / 100)
                     }
                     valueLabelDisplay="auto"
                     max={100}
                     min={0}
                     disabled={audioInfo.muted}
                     aria-label="Volume"
                  />
               </Box>
            )}
         </div>
         <Divider />
         <MenuItem>
            <ListItemIcon style={{ minWidth: 32 }}>
               <SendIcon fontSize="small" />
            </ListItemIcon>
            Send private message
         </MenuItem>
         <MenuItem>Kick</MenuItem>
         <MenuItem>Pause microphone for all</MenuItem>
         <div
            style={{
               display: 'flex',
               justifyContent: 'space-between',
               paddingLeft: 16,
               paddingRight: 16,
               paddingTop: 8,
               paddingBottom: 8,
               alignItems: 'center',
            }}
         >
            <Typography variant="subtitle1">Permissions:</Typography>
            <ToggleButtonGroup aria-label="temporary permissions" size="small">
               <ToggleButton value="screen" aria-label="allow screen sharing">
                  <DesktopWindowsIcon fontSize="small" />
               </ToggleButton>
               <ToggleButton value="webcam" aria-label="allow webcam">
                  <VideocamIcon fontSize="small" />
               </ToggleButton>
               <ToggleButton value="mic" aria-label="allow microphone">
                  <MicIcon fontSize="small" />
               </ToggleButton>
            </ToggleButtonGroup>
         </div>
         <MenuItem onClick={handleShowPermissions}>Show Permissions</MenuItem>
      </>
   );
});

ParticipantContextMenu.displayName = 'ParticipantContextMenu';

export default ParticipantContextMenu;
