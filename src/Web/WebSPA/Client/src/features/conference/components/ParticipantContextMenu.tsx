import { Box, Divider, IconButton, ListItemIcon, makeStyles, MenuItem, Slider, Typography } from '@material-ui/core';
import MicOffRounded from '@material-ui/icons/MicOff';
import SendIcon from '@material-ui/icons/Send';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import _ from 'lodash';
import { AccountRemove } from 'mdi-material-ui';
import React, { useCallback, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { openPrivateChat, setSelectedChannel } from 'src/features/chat/reducer';
import { selectPrivateMessageEnabled } from 'src/features/chat/selectors';
import { createPrivatChatChannel } from 'src/features/chat/utils';
import { patchParticipantAudio } from 'src/features/media/reducer';
import { selectParticipantAudioInfo } from 'src/features/media/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import {
   CONFERENCE_CAN_KICK_PARTICIPANT,
   MEDIA_CAN_CHANGE_PARTICIPANTS_PRODUCER,
   PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION,
   PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS,
   SCENES_CAN_SET_SCENE,
} from 'src/permissions';
import { RootState } from 'src/store';
import { showMessage } from 'src/store/notifier/actions';
import { Participant } from '../types';
import ParticipantContextMenuTempPermissions from './ParticipantContextMenuTempPermissions';
import { AccountVoice } from 'mdi-material-ui';

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
   const dispatch = useDispatch();
   const classes = useStyles();
   const { t } = useTranslation();

   const audioInfo = useSelector((state: RootState) => selectParticipantAudioInfo(state, participant.id));
   const myId = useMyParticipantId();

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
      }, 1000),
      [participant.id],
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
      const privateChatId = createPrivatChatChannel(participant.id, myId);

      dispatch(openPrivateChat(privateChatId));
      dispatch(setSelectedChannel(privateChatId));
      onClose();
   };

   const handleKickParticipant = () => {
      dispatch(
         showMessage({
            type: 'loading',
            message: t('conference.notifications.kick.loading', { name: participant.displayName }),
            dismissOn: {
               type: coreHub.kickParticipant.returnAction,
               successMessage: t('conference.notifications.kick.success', { name: participant.displayName }),
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
            message: t('conference.notifications.disable_mic.loading', { name: participant.displayName }),
            dismissOn: {
               type: coreHub.changeProducerSource.returnAction,
               successMessage: t('conference.notifications.disable_mic.success', { name: participant.displayName }),
            },
         }),
      );
      dispatch(coreHub.changeProducerSource({ participantId: participant.id, source: 'mic', action: 'close' }));
      onClose();
   };

   const handleSetPresenter = () => {
      dispatch(coreHub.setScene({ type: 'presenter', presenterParticipantId: participant.id }));
      onClose();
   };

   const canSetTempPermission = usePermission(PERMISSIONS_CAN_GIVE_TEMPORARY_PERMISSION);
   const canSeePermissions = usePermission(PERMISSIONS_CAN_SEE_ANY_PARTICIPANTS_PERMISSIONS);
   const canSendPrivateMessage = useSelector(selectPrivateMessageEnabled);
   const canKick = usePermission(CONFERENCE_CAN_KICK_PARTICIPANT);
   const canChangeParticipantProducers = usePermission(MEDIA_CAN_CHANGE_PARTICIPANTS_PRODUCER);
   const canSetScene = usePermission(SCENES_CAN_SET_SCENE);

   return (
      <>
         <div className={classes.infoMenuItem}>
            <Typography>{participant.displayName}</Typography>
            {audioInfo && (
               <Box display="flex" alignItems="center" style={{ marginTop: 4, marginBottom: 4 }}>
                  <IconButton
                     size="small"
                     onClick={handleToggleMuted}
                     aria-label={
                        audioInfo.muted
                           ? t('conference.participant_context_menu.unmute')
                           : t('conference.participant_context_menu.mute')
                     }
                  >
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
                     aria-label={t('common:volume')}
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
               {t('conference.participant_context_menu.open_private_chat')}
            </MenuItem>
         )}
         {canKick && (
            <MenuItem onClick={handleKickParticipant}>
               <ListItemIcon className={classes.menuIcon}>
                  <AccountRemove fontSize="small" />
               </ListItemIcon>
               {t('conference.participant_context_menu.kick')}
            </MenuItem>
         )}
         {canChangeParticipantProducers && (
            <MenuItem onClick={handleMuteParticipant}>
               <ListItemIcon className={classes.menuIcon}>
                  <MicOffRounded fontSize="small" />
               </ListItemIcon>
               {t('conference.participant_context_menu.disable_mic_for_all')}
            </MenuItem>
         )}
         {canSetScene && (
            <MenuItem onClick={handleSetPresenter}>
               <ListItemIcon className={classes.menuIcon}>
                  <AccountVoice fontSize="small" />
               </ListItemIcon>
               {t('conference.participant_context_menu.make_presenter')}
            </MenuItem>
         )}
         {canSetTempPermission && <ParticipantContextMenuTempPermissions participantId={participant.id} />}
         {canSeePermissions && (
            <MenuItem onClick={handleShowPermissions}>
               {t('conference.participant_context_menu.show_permissions')}
            </MenuItem>
         )}
      </>
   );
});

ParticipantContextMenu.displayName = 'ParticipantContextMenu';

export default ParticipantContextMenu;
