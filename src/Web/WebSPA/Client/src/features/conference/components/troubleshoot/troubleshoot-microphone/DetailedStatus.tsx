import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import CheckIcon from '@material-ui/icons/Check';
import CloseIcon from '@material-ui/icons/Close';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectParticipantProducers } from 'src/features/media/selectors';
import { selectAvailableInputDevices } from 'src/features/settings/selectors';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { RootState } from 'src/store';
import useConsumer from 'src/store/webrtc/hooks/useConsumer';
import { findMicrophoneLabel, getDefaultDevice } from './utilts';

type StatusListItemProps = {
   success?: boolean;
   description: string;
   secondary?: string;
};

function StatusListItem({ success, description, secondary }: StatusListItemProps) {
   return (
      <ListItem>
         <ListItemIcon>{success ? <CheckIcon /> : <CloseIcon />}</ListItemIcon>
         <ListItemText primary={description} secondary={secondary} />
      </ListItem>
   );
}

type Props = {
   enableError?: string | null;
};

export default function DetailedStatus({ enableError }: Props) {
   const { t } = useTranslation();
   const mics = useSelector((state: RootState) => selectAvailableInputDevices(state, 'mic'));
   const audioDevice = useSelector((state: RootState) => state.settings.obj.mic.device) ?? getDefaultDevice(mics);

   const myId = useMyParticipantId();
   const consumer = useConsumer(myId, 'loopback-mic');

   const streams = useSelector((state: RootState) => selectParticipantProducers(state, myId));
   const loopbackMicStream = streams?.['loopback-mic'];

   return (
      <div>
         <List dense disablePadding>
            {audioDevice ? (
               <StatusListItem
                  success
                  description={t('conference.troubleshooting.microphone.device.success')}
                  secondary={findMicrophoneLabel(audioDevice, mics)}
               />
            ) : (
               <StatusListItem description={t('conference.troubleshooting.microphone.device.not_found')} />
            )}
            {!enableError ? (
               <StatusListItem success description={t('conference.troubleshooting.microphone.activation.success')} />
            ) : (
               <StatusListItem
                  description={t('conference.troubleshooting.microphone.activation.error')}
                  secondary={enableError}
               />
            )}
            {loopbackMicStream?.paused === false ? (
               <StatusListItem success description={t('conference.troubleshooting.microphone.stream.success')} />
            ) : (
               <StatusListItem
                  description={t('conference.troubleshooting.microphone.stream.error')}
                  secondary={
                     loopbackMicStream
                        ? t('conference.troubleshooting.microphone.stream.error_paused')
                        : t('conference.troubleshooting.microphone.stream.error_not_found')
                  }
               />
            )}
            {consumer ? (
               <StatusListItem success description={t('conference.troubleshooting.microphone.consumer.success')} />
            ) : (
               <StatusListItem description={t('conference.troubleshooting.microphone.consumer.not_found')} />
            )}
         </List>
      </div>
   );
}
