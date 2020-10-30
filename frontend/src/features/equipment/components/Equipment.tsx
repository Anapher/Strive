import { Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { fetchDevices } from '../../settings/thunks';
import { getDeviceName } from '../ua-utils';
import AvailableEquipmentTable from './AvailableEquipmentTable';
import * as coreHub from 'src/core-hub';
import useMicrophone from 'src/store/webrtc/hooks/useMicrophone';
import { ProducerSource } from 'src/store/webrtc/types';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';
import { mapValues } from 'src/utils/obj-utils';

const deviceName = getDeviceName();

export default function Equipment() {
   const dispatch = useDispatch();
   const availableEquipment = useSelector((state: RootState) => state.settings.availableDevices);
   const currentCommand = useSelector((state: RootState) => state.equipment.command);

   useEffect(() => {
      dispatch(fetchDevices());
   }, []);

   useEffect(() => {
      if (availableEquipment) {
         dispatch(coreHub.registerEquipment({ name: deviceName, devices: availableEquipment }));
      }
   }, [availableEquipment]);

   const mic = useMicrophone();

   const controllers: { [src in ProducerSource]: UseMediaState } = { mic, screen: mic, webcam: mic };

   useEffect(() => {
      // execute current command if changed
      if (currentCommand) {
         const control = controllers[currentCommand.source];

         const executeCommand = async () => {
            try {
               switch (currentCommand.action) {
                  case 'enable':
                     await control.enable();
                     break;
                  case 'disable':
                     control.disable();
                     break;
                  case 'pause':
                     control.pause();
                     break;
                  case 'resume':
                     control.resume();
                     break;
                  case 'switchDevice':
                     await control.switchDevice(currentCommand.deviceId);
                     break;
               }
            } catch (error) {
               dispatch(
                  coreHub.equipmentErrorOccurred({ message: error.toString(), code: -1, type: 'EquipmentError' }),
               );
            }
         };

         executeCommand();
      }
   }, [currentCommand]);

   // update current status if changed
   useEffect(
      () => {
         const status = mapValues(controllers, ({ connected, enabled, paused, streamInfo }) => ({
            connected,
            enabled,
            paused,
            streamInfo,
         }));

         dispatch(coreHub.equipmentUpdateStatus(status));
      },
      Object.values(controllers).reduce<Array<any>>(
         (prev, curr) => [...prev, curr.connected, curr.enabled, curr.paused, curr.streamInfo],
         [],
      ),
   );

   return (
      <div>
         <Typography variant="h4">{deviceName}</Typography>
         <Typography>{JSON.stringify(currentCommand)}</Typography>
         <AvailableEquipmentTable />
      </div>
   );
}
