import { Divider, Grid, ListItemIcon, makeStyles, MenuItem, Slider, Typography } from '@material-ui/core';
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

const useStyles = makeStyles((theme) => ({
   infoMenuItem: {
      padding: theme.spacing(0, 1),
   },
}));

type Props = {
   participant: ParticipantDto;
};

const ParticipantContextMenu = React.forwardRef<HTMLElement, Props>(({ participant }, ref) => {
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

   return (
      <>
         <div className={classes.infoMenuItem}>
            <Typography>{participant.displayName}</Typography>
            {audioInfo && (
               <Grid container spacing={2} alignItems="center">
                  <Grid item>
                     {audioInfo.muted ? <VolumeMuteIcon fontSize="small" /> : <VolumeUpIcon fontSize="small" />}
                  </Grid>
                  <Grid item xs>
                     <Slider
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
                  </Grid>
               </Grid>
            )}
         </div>
         <Divider />
         <MenuItem>
            <ListItemIcon style={{ minWidth: 32 }}>
               <VolumeMuteIcon fontSize="small" />
            </ListItemIcon>
            <Typography variant="inherit">Mute</Typography>
         </MenuItem>
         <MenuItem>Show info for nerds</MenuItem>
         <Typography variant="subtitle1" gutterBottom>
            Temporary Permissions
         </Typography>
         <ToggleButtonGroup aria-label="temporary permissions" size="small">
            <ToggleButton value="webcam" aria-label="allow webcam">
               <VideocamIcon />
            </ToggleButton>
            <ToggleButton value="screen" aria-label="allow screen sharing">
               <DesktopWindowsIcon />
            </ToggleButton>
            <ToggleButton value="mic" aria-label="allow microphone">
               <MicIcon />
            </ToggleButton>
         </ToggleButtonGroup>
      </>
   );
});

ParticipantContextMenu.displayName = 'ParticipantContextMenu';

export default ParticipantContextMenu;
