import { Box, Divider, IconButton, ListItemIcon, makeStyles, MenuItem, Slider, Typography } from '@material-ui/core';
import SendIcon from '@material-ui/icons/Send';
import VolumeMuteIcon from '@material-ui/icons/VolumeMute';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import _ from 'lodash';
import React, { useCallback } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchPermissions } from 'src/core-hub';
import { setSendingMode } from 'src/features/chat/reducer';
import { patchParticipantAudio } from 'src/features/media/reducer';
import { selectParticipantAudioInfo } from 'src/features/media/selectors';
import usePermission, {
   CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE,
   PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION,
   PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS,
} from 'src/hooks/usePermission';
import { RootState } from 'src/store';
import { ParticipantDto } from '../types';
import ParticipantContextMenuTempPermissions from './ParticipantContextMenuTempPermissions';

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

   const handleSendPrivateMessage = () => {
      dispatch(
         setSendingMode({
            type: 'privately',
            to: { participantId: participant.participantId, displayName: participant.displayName },
         }),
      );
      onClose();
   };

   const canSetTempPermission = usePermission(PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION);
   const canSeePermissions = usePermission(PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS);
   const canSendPrivateMessage = usePermission(CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE);

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
         {canSendPrivateMessage && (
            <MenuItem onClick={handleSendPrivateMessage}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <SendIcon fontSize="small" />
               </ListItemIcon>
               Send private message
            </MenuItem>
         )}
         <MenuItem>Kick</MenuItem>
         <MenuItem>Pause microphone for all</MenuItem>
         {canSetTempPermission && <ParticipantContextMenuTempPermissions participantId={participant.participantId} />}
         {canSeePermissions && <MenuItem onClick={handleShowPermissions}>Show Permissions</MenuItem>}
      </>
   );
});

ParticipantContextMenu.displayName = 'ParticipantContextMenu';

export default ParticipantContextMenu;
