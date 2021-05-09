import { makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { fetchDevices } from '../../settings/thunks';
import { getDeviceName } from '../ua-utils';
import AvailableEquipmentTable from './AvailableEquipmentTable';
import * as equipmentHub from 'src/equipment-hub';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import { ProducerSource } from 'src/store/webrtc/types';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';
import { mapValues } from 'src/utils/obj-utils';
import * as errors from 'src/errors';
import CommandHistory from './CommandHistory';
import { commandExecuted } from '../reducer';
import useWebcam from 'src/store/webrtc/hooks/useWebcam';
import useScreen from 'src/store/webrtc/hooks/useScreen';
import { DomainError } from 'src/communication-types';

const useStyles = makeStyles((theme) => ({
   root: {
      marginLeft: 'auto',
      marginRight: 'auto',
      maxWidth: 800,
      padding: theme.spacing(2),
   },
   historyContainer: {
      maxHeight: '80vh',
      overflowY: 'auto',
   },
}));

const deviceName = getDeviceName();

export default function Equipment() {
   const dispatch = useDispatch();
   const classes = useStyles();
   const executingCommands = useRef(new Set<number>());

   const availableEquipment = useSelector((state: RootState) => state.settings.availableDevices);
   const commandHistory = useSelector((state: RootState) => state.equipment.commandHistory);

   useEffect(() => {
      dispatch(fetchDevices());
   }, []);

   useEffect(() => {
      if (availableEquipment) {
         dispatch(equipmentHub.initialize({ name: deviceName, devices: availableEquipment }));
      }
   }, [availableEquipment]);

   const mic = useMicrophone();
   const loopbackMic = useMicrophone(undefined, true);

   const webcam = useWebcam();
   const loopbackWebcam = useWebcam(true);

   const screen = useScreen();

   const controllers: { [src in ProducerSource]: UseMediaState } = {
      mic,
      webcam,
      screen,
      ['loopback-mic']: loopbackMic,
      ['loopback-webcam']: loopbackWebcam,
      ['loopback-screen']: screen,
   };

   useEffect(() => {
      const newCommands = commandHistory.filter((x) => !x.executed && !executingCommands.current.has(x.id));

      for (const { command, id } of newCommands) {
         executingCommands.current.add(id);

         const control = controllers[command.source];
         const executeCommand = async () => {
            try {
               switch (command.action) {
                  case 'enable':
                     await control.enable();
                     break;
                  case 'disable':
                     await control.disable();
                     break;
                  case 'pause':
                     await control.pause();
                     break;
                  case 'resume':
                     await control.resume();
                     break;
                  case 'switchDevice':
                     await control.switchDevice(command.deviceId);
                     break;
               }
               dispatch(commandExecuted({ id }));
            } catch (err) {
               const error: DomainError = errors.equipmentCommandError(
                  err.message || err.toString(),
                  command.action,
                  command.source,
               );

               dispatch(commandExecuted({ id, error }));
               dispatch(equipmentHub.errorOccurred(error));
            } finally {
               executingCommands.current.delete(id);
            }
         };

         executeCommand();
      }
   }, [commandHistory]);

   // update current status if changed
   useEffect(
      () => {
         const status = mapValues(controllers, ({ connected, enabled, paused, streamInfo }) => ({
            connected,
            enabled,
            paused,
            streamInfo,
         }));

         dispatch(equipmentHub.updateStatus(status));
      },
      Object.values(controllers).reduce<Array<any>>(
         (prev, curr) => [...prev, curr.connected, curr.enabled, curr.paused, curr.streamInfo],
         [],
      ),
   );

   return (
      <div className={classes.root}>
         <Typography align="center" variant="h4" gutterBottom>
            {deviceName}
         </Typography>
         <AvailableEquipmentTable />
         <div className={classes.historyContainer}>
            <CommandHistory />
         </div>
      </div>
   );
}
