import _ from 'lodash';
import { DeviceGroup } from 'src/features/settings/selectors';
import { AnyInputDevice } from 'src/features/settings/types';

export const findMicrophoneLabel = (device: AnyInputDevice, deviceGroup: DeviceGroup[]) =>
   device.type === 'local'
      ? deviceGroup.find((x) => x.type === 'local')?.devices.find((x) => x.device.deviceId === device.deviceId)?.label
      : deviceGroup
           .find((x) => x.type === 'equipment' && x.connectionId === device.connectionId)
           ?.devices.find((x) => x.device.deviceId === device.deviceId)?.label;

export const getDefaultDevice = (deviceGroup: DeviceGroup[]) => {
   return _.first(deviceGroup.find((x) => x.type === 'local')?.devices)?.device;
};
