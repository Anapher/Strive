import _ from 'lodash';
import { Dispatch, useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { sendEquipmentCommand } from 'src/core-hub';
import { EquipmentCommandAction } from 'src/equipment-hub.types';
import { RootState } from 'src/store';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';
import { ProducerSource } from 'src/store/webrtc/types';
import { AnyInputDevice } from '../settings/types';
import { EquipmentConnection } from './types';

function wrapControl(
   source: ProducerSource,
   device: AnyInputDevice | undefined,
   local: UseMediaState,
   equipment: Record<string, EquipmentConnection> | undefined,
   dispatch: Dispatch<any>,
): UseMediaState {
   if (!device || device.type === 'local') {
      return local;
   } else {
      const equipmentInfo = equipment?.[device.connectionId]?.status?.[source] ?? {
         connected: false,
         enabled: false,
         paused: false,
      };

      const executeEquipmentCommand = (action: EquipmentCommandAction) => {
         dispatch(
            sendEquipmentCommand({ action, connectionId: device.connectionId, source, deviceId: device.deviceId }),
         );
      };

      return {
         enable: () => executeEquipmentCommand('enable'),
         disable: () => executeEquipmentCommand('disable'),
         pause: () => executeEquipmentCommand('pause'),
         resume: () => executeEquipmentCommand('resume'),
         switchDevice: (deviceId) => {
            dispatch(
               sendEquipmentCommand({
                  action: 'switchDevice',
                  connectionId: device.connectionId,
                  source,
                  deviceId,
               }),
            );
         },
         ...equipmentInfo,
      };
   }
}

export default function useDeviceManagement(
   source: ProducerSource,
   local: UseMediaState,
   device?: AnyInputDevice,
): UseMediaState {
   const currentDevice = useRef<AnyInputDevice | undefined>();
   const dispatch = useDispatch();
   const equipment = useSelector((state: RootState) => state.media.equipment?.connections);

   useEffect(() => {
      console.log('source', source);

      console.log('new device', device);
      console.log('previous device', currentDevice.current);

      if (_.isEqual(currentDevice.current, device)) return;

      const deviceType = device?.type || 'local'; // undefined device is default device locally
      const previousDeviceType = currentDevice.current?.type || 'local';

      // disable previous device
      if (previousDeviceType !== deviceType) {
         if (previousDeviceType === 'local') {
            local.disable();
            console.log('disable local device');
         } else if (currentDevice.current?.type === 'equipment') {
            console.log('disable remote device');

            dispatch(
               sendEquipmentCommand({
                  action: 'disable',
                  connectionId: currentDevice.current.connectionId,
                  source,
                  deviceId: currentDevice.current.deviceId,
               }),
            );
         }
      }

      if (device?.type === 'equipment') {
         dispatch(
            sendEquipmentCommand({
               action: 'switchDevice',
               connectionId: device.connectionId,
               source,
               deviceId: device.deviceId,
            }),
         );
      } else {
         local.switchDevice(device?.deviceId);
      }

      currentDevice.current = device;
   }, [device]);

   const controller = wrapControl(source, device, local, equipment, dispatch);

   useEffect(() => {
      return () => {
         if (currentDevice.current?.type === 'equipment') {
            console.log('unmount, disable equipment device');
            dispatch(
               sendEquipmentCommand({
                  action: 'disable',
                  connectionId: currentDevice.current.connectionId,
                  source,
                  deviceId: currentDevice.current.deviceId,
               }),
            );
         }
      };
   }, []);

   return controller;
}
