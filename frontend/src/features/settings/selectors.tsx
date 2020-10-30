import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { ProducerSource } from 'src/store/webrtc/types';
import { AnyInputDevice } from './types';

const getSource = (_: unknown, source: ProducerSource | undefined) => source;
const getLocalDevices = (state: RootState) => state.settings.availableDevices;
const getEquipment = (state: RootState) => state.media.equipment;

export const selectAvailableInputDevices = createSelector(
   getLocalDevices,
   getEquipment,
   getSource,
   (devices, equipment, source) => {
      const result = new Array<DeviceGroup>();

      if (devices) {
         result.push({
            type: 'local',
            devices: devices
               .filter((x) => x.source === source)
               .map((x) => ({ label: x.label, device: { type: 'local', deviceId: x.deviceId } })),
         });
      }

      if (equipment) {
         for (const connected of equipment) {
            if (connected.devices)
               result.push({
                  type: 'equipment',
                  equipmentName: connected.name,
                  equipmentId: connected.equipmentId,
                  devices: connected.devices
                     ?.filter((x) => x.source === source)
                     .map((x) => ({
                        label: x.label,
                        device: { type: 'equipment', deviceId: x.deviceId, equipmentId: connected.equipmentId },
                     })),
               });
         }
      }

      return result;
   },
);

export type DeviceGroup = EquipmentDeviceGroup | LocalDeviceGroup;

export type EquipmentDeviceGroup = {
   type: 'equipment';
   equipmentId: string;
   equipmentName?: string;
   devices: DeviceViewModel[];
};

export type LocalDeviceGroup = {
   type: 'local';
   devices: DeviceViewModel[];
};

export type DeviceViewModel = {
   device: AnyInputDevice;
   label?: string;
};
