import { List, ListItem, ListItemIcon, ListItemText, ListSubheader, Typography } from '@material-ui/core';
import { TFunction } from 'i18next';
import { Microphone, MicrophoneOff, Monitor, MonitorOff, Video, VideoOff } from 'mdi-material-ui';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { EquipmentCommand, EquipmentCommandAction } from 'src/equipment-hub.types';
import { selectLocalDevices } from 'src/features/settings/selectors';
import { InputDeviceDto } from 'src/features/settings/types';
import { RootState } from 'src/store';
import { ProducerSource } from 'src/store/webrtc/types';

function ActionIcon(source: ProducerSource, action: EquipmentCommandAction) {
   switch (source) {
      case 'mic':
      case 'loopback-mic':
         switch (action) {
            case 'disable':
               return <MicrophoneOff />;
            default:
               return <Microphone />;
         }
      case 'webcam':
      case 'loopback-webcam':
         switch (action) {
            case 'disable':
               return <VideoOff />;
            default:
               return <Video />;
         }
      case 'screen':
      case 'loopback-screen':
         switch (action) {
            case 'disable':
               return <MonitorOff />;
            default:
               return <Monitor />;
         }
      default:
         break;
   }
}

export default function CommandHistory() {
   const { t } = useTranslation();
   const history = useSelector((state: RootState) => state.equipment.commandHistory);
   const devices = useSelector(selectLocalDevices);

   return (
      <List
         aria-labelledby="command-history-subheader"
         subheader={
            <ListSubheader component="div" id="command-history-subheader" disableSticky>
               {t('conference.equipment.command_history')}
            </ListSubheader>
         }
      >
         {history.map(({ id, command, executed, error }) => (
            <ListItem key={id}>
               <ListItemIcon>{ActionIcon(command.source, command.action)}</ListItemIcon>
               <ListItemText
                  primary={getHistoryListPrimary(t, command, devices)}
                  secondary={
                     !executed
                        ? t('conference.equipment.history.pending')
                        : !error
                        ? t('conference.equipment.history.succeeded')
                        : t('conference.equipment.history.error', { error: error.fields?.message || error.message })
                  }
               />
            </ListItem>
         ))}
      </List>
   );
}

function getHistoryListPrimary(t: TFunction, command: EquipmentCommand, devices: InputDeviceDto[] | null) {
   const sourceTranslation = t(`conference.equipment.history.sources.${command.source}`);
   const deviceName = command.deviceId && devices?.find((x) => x.deviceId === command.deviceId)?.label;
   const actionTranslation = t(`conference.equipment.history.actions.${command.action}`, {
      name: deviceName,
   });

   return (
      <>
         {sourceTranslation} | <Typography variant="caption">{actionTranslation}</Typography>
      </>
   );
}
