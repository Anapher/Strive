import { Box, Button, Grid, makeStyles, Slider, Typography } from '@material-ui/core';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';
import { MotionValue } from 'framer-motion';
import React from 'react';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import VideocamIcon from '@material-ui/icons/Videocam';
import MicIcon from '@material-ui/icons/Mic';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import VolumeMuteIcon from '@material-ui/icons/VolumeMute';
import { ParticipantDto } from '../types';

const useStyles = makeStyles({
   muteButton: {
      width: 80,
   },
});

type Props = {
   audioLevel: MotionValue<number>;
   participant: ParticipantDto;
   muted: boolean;
   onChangeMuted: (mute: boolean) => void;
   volume: number;
   onChangeVolume: (volume: number) => void;
};

export default function ParticipantItemPopper({ participant, volume, onChangeVolume, muted, onChangeMuted }: Props) {
   const classes = useStyles();

   const handleVolumeChange = (event: any, newValue: number | number[]) => {
      onChangeVolume((newValue as number) / 100);
   };

   const handleToggleMute = () => {
      onChangeMuted(!muted);
   };

   return (
      <div>
         <Typography variant="h6" gutterBottom>
            {participant.displayName}
         </Typography>
         <Grid container spacing={2} alignItems="center">
            <Grid item>{muted ? <VolumeMuteIcon /> : <VolumeUpIcon />}</Grid>
            <Grid item xs>
               <Slider
                  value={muted ? 0 : Math.round(volume * 100)}
                  onChange={handleVolumeChange}
                  valueLabelDisplay="auto"
                  max={100}
                  min={0}
                  disabled={muted}
                  aria-label="Volume"
               />
            </Grid>
            <Grid item>
               <Button
                  onClick={handleToggleMute}
                  className={classes.muteButton}
                  variant="contained"
                  size="small"
                  color={muted ? 'default' : 'primary'}
               >
                  {muted ? 'Unmute' : 'Mute'}
               </Button>
            </Grid>
         </Grid>

         <Box mt={2}>
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
         </Box>
      </div>
   );
}
