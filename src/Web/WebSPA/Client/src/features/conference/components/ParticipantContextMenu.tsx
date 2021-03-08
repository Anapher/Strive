import { Box, Divider, IconButton, ListItemIcon, makeStyles, MenuItem, Slider, Typography } from '@material-ui/core';
import MicOffRounded from '@material-ui/icons/MicOff';
import SendIcon from '@material-ui/icons/Send';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import _ from 'lodash';
import { AccountRemove } from 'mdi-material-ui';
import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { setSendingMode } from 'src/features/chat/reducer';
import { patchParticipantAudio } from 'src/features/media/reducer';
import { selectParticipantAudioInfo } from 'src/features/media/selectors';
import usePermission from 'src/hooks/usePermission';
import {
   CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE,
   CONFERENCE_CAN_KICK_PARTICIPANT,
   MEDIA_CAN_CHANGE_PARTICIPANTS_PRODUCER,
   PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION,
   PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS,
} from 'src/permissions';
import { RootState } from 'src/store';
import { showMessage } from 'src/store/notifier/actions';
import { Participant } from '../types';
import ParticipantContextMenuTempPermissions from './ParticipantContextMenuTempPermissions';

const useStyles = makeStyles((theme) => ({
   infoMenuItem: {
      padding: theme.spacing(0, 1),
   },
   menuIcon: {
      minWidth: 32,
   },
}));

type Props = {
   participant: Participant;
   onClose: () => void;
};

const ParticipantContextMenu = React.forwardRef<HTMLElement, Props>(({ participant, onClose }) => {
   const audioInfo = useSelector((state: RootState) => selectParticipantAudioInfo(state, participant.id));
   const dispatch = useDispatch();
   const classes = useStyles();

   const handleToggleMuted = () => {
      dispatch(patchParticipantAudio({ participantId: participant.id, data: { muted: !audioInfo?.muted } }));
   };

   // hold volume as state because we update the global state in throttle mode which
   // would result in a lagging slider if this would directly be used
   const [displayedVolume, setDisplayedVolume] = useState(audioInfo?.volume ?? 0.75);

   useEffect(() => {
      if (audioInfo) {
         setDisplayedVolume(audioInfo.volume);
      }
   }, [audioInfo?.volume]);

   const handlePatchVolume = useCallback(
      _.throttle((volume: number) => {
         dispatch(patchParticipantAudio({ participantId: participant.id, data: { volume } }));
      }, 500),
      [participant],
   );

   const handleChangeVolume = (value: number) => {
      if (audioInfo?.muted) {
         handleToggleMuted();
      }

      setDisplayedVolume(value);
      handlePatchVolume(value);
   };

   const handleShowPermissions = () => {
      dispatch(coreHub.fetchPermissions(participant.id));
      onClose();
   };

   const handleSendPrivateMessage = () => {
      dispatch(
         setSendingMode({
            type: 'privately',
            to: { participantId: participant.id, displayName: participant.displayName },
         }),
      );
      onClose();
   };

   const handleKickParticipant = () => {
      dispatch(
         showMessage({
            type: 'loading',
            message: `Kick ${participant.displayName}...`,
            dismissOn: {
               type: coreHub.kickParticipant.returnAction,
               successMessage: `${participant.displayName} was removed from conference`,
            },
         }),
      );
      dispatch(coreHub.kickParticipant({ participantId: participant.id }));
      onClose();
   };

   const handleMuteParticipant = () => {
      dispatch(
         showMessage({
            type: 'loading',
            message: `Disable microphone of ${participant.displayName}`,
            dismissOn: {
               type: coreHub.changeProducerSource.returnAction,
               successMessage: 'Microphone was disabled',
            },
         }),
      );
      dispatch(coreHub.changeProducerSource({ participantId: participant.id, source: 'mic', action: 'close' }));
      onClose();
   };

   const canSetTempPermission = usePermission(PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION);
   const canSeePermissions = usePermission(PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS);
   const canSendPrivateMessage = usePermission(CHAT_CAN_SEND_PRIVATE_CHAT_MESSAGE);
   const canKick = usePermission(CONFERENCE_CAN_KICK_PARTICIPANT);
   const canChangeParticipantProducers = usePermission(MEDIA_CAN_CHANGE_PARTICIPANTS_PRODUCER);

   return (
      <>
         <div className={classes.infoMenuItem}>
            <Typography>{participant.displayName}</Typography>
            {audioInfo && (
               <Box display="flex" alignItems="center" style={{ marginTop: 4, marginBottom: 4 }}>
                  <IconButton size="small" onClick={handleToggleMuted}>
                     {audioInfo.muted ? <VolumeOffIcon fontSize="small" /> : <VolumeUpIcon fontSize="small" />}
                  </IconButton>
                  <Slider
                     style={{ flex: 1, marginLeft: 8 }}
                     value={audioInfo.muted ? 0 : Math.round(displayedVolume * 100)}
                     onChange={(_event: unknown, newValue: number | number[]) => {
                        handleChangeVolume((newValue as number) / 100);
                     }}
                     valueLabelDisplay="auto"
                     max={100}
                     min={0}
                     aria-label="Volume"
                  />
               </Box>
            )}
         </div>
         <Divider />
         {canSendPrivateMessage && (
            <MenuItem onClick={handleSendPrivateMessage}>
               <ListItemIcon className={classes.menuIcon}>
                  <SendIcon fontSize="small" />
               </ListItemIcon>
               Send private message
            </MenuItem>
         )}
         {canKick && (
            <MenuItem onClick={handleKickParticipant}>
               <ListItemIcon className={classes.menuIcon}>
                  <AccountRemove fontSize="small" />
               </ListItemIcon>
               Kick
            </MenuItem>
         )}
         {canChangeParticipantProducers && (
            <MenuItem onClick={handleMuteParticipant}>
               <ListItemIcon className={classes.menuIcon}>
                  <MicOffRounded fontSize="small" />
               </ListItemIcon>
               Disable microphone for all
            </MenuItem>
         )}
         {canSetTempPermission && <ParticipantContextMenuTempPermissions participantId={participant.id} />}
         {canSeePermissions && <MenuItem onClick={handleShowPermissions}>Show Permissions</MenuItem>}
      </>
   );
});

ParticipantContextMenu.displayName = 'ParticipantContextMenu';

export default ParticipantContextMenu;
