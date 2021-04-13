import { List, ListItem, ListItemIcon, ListItemText, ListSubheader } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from 'src/store';
import VideocamIcon from '@material-ui/icons/Videocam';
import MicIcon from '@material-ui/icons/Mic';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import { useTranslation } from 'react-i18next';

export default function AvailableEquipmentTable() {
   const devices = useSelector((state: RootState) => state.settings.availableDevices);
   const { t } = useTranslation();

   return (
      <List
         aria-labelledby="available-devices-subheader"
         subheader={
            <ListSubheader component="div" id="available-devices-subheader">
               {t('conference.equipment.available_devices')}
            </ListSubheader>
         }
      >
         {devices &&
            devices.map((x) => (
               <ListItem key={x.deviceId}>
                  <ListItemIcon>
                     {x.source === 'mic' ? (
                        <MicIcon />
                     ) : x.source === 'screen' ? (
                        <DesktopWindowsIcon />
                     ) : (
                        <VideocamIcon />
                     )}
                  </ListItemIcon>
                  <ListItemText primary={x.label} />
               </ListItem>
            ))}
      </List>
   );
}
