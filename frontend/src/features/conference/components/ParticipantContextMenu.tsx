import {
   Divider,
   Grid,
   ListItemIcon,
   makeStyles,
   Menu,
   MenuItem,
   MenuList,
   Slider,
   Typography,
} from '@material-ui/core';
import React, { useCallback } from 'react';
import { ParticipantDto } from '../types';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import VolumeMuteIcon from '@material-ui/icons/VolumeMute';
import { selectParticipantAudioInfo } from 'src/features/media/selectors';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { patchParticipantAudio } from 'src/features/media/mediaSlice';
import _ from 'lodash';

const useStyles = makeStyles((theme) => ({
   infoMenuItem: {
      padding: theme.spacing(0, 1),
   },
}));

type Props = {
   participant: ParticipantDto;
};

export default function ParticipantContextMenu({ participant }: Props) {
   const audioInfo = useSelector((state: RootState) => selectParticipantAudioInfo(state, participant.participantId));
   const dispatch = useDispatch();
   const classes = useStyles();

   const handleChangeMuted = (muted: boolean) => {
      dispatch(patchParticipantAudio({ participantId: participant.participantId, data: { muted } }));
   };

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
         <MenuItem>Logout</MenuItem>
      </>
   );
}
