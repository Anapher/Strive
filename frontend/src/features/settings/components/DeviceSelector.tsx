import { FormControl, InputLabel, ListSubheader, makeStyles, MenuItem, Select } from '@material-ui/core';
import clsx from 'classnames';
import _, { Collection } from 'lodash';
import React from 'react';
import { DeviceGroup, EquipmentDeviceGroup } from '../selectors';
import { AnyInputDevice } from '../types';

const useStyles = makeStyles({
   control: {
      minWidth: 400,
   },
});

type Props = {
   devices: DeviceGroup[];
   defaultName: string;
   className?: string;
   label: string;

   onChange: (device: AnyInputDevice) => void;

   selectedDevice?: AnyInputDevice;
};

const getId = (device: AnyInputDevice) =>
   device.type === 'local' ? device.deviceId : `${device.equipmentId}/${device.deviceId}`;

export default function DeviceSelector({ devices, defaultName, className, selectedDevice, onChange, label }: Props) {
   const selectId = defaultName.toLowerCase() + '-select';
   const classes = useStyles();

   const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
      const value = event.target.value as string;
      let deviceId: string | undefined;
      let possibleDevices: Collection<DeviceGroup> | undefined;

      if (value.includes('/')) {
         const split = value.split('/');
         possibleDevices = _(devices).filter((x) => x.type === 'equipment' && x.equipmentId === split[0]);
         deviceId = split[1];
      } else {
         possibleDevices = _(devices).filter((x) => x.type === 'local');
         deviceId = value;
      }

      const viewModel = possibleDevices.flatMap((x) => x.devices).find((x) => x.device.deviceId === deviceId);
      if (viewModel) {
         onChange(viewModel.device);
      }
   };

   return (
      <FormControl className={clsx(className, classes.control)}>
         <InputLabel htmlFor={selectId}>{label}</InputLabel>
         <Select id={selectId} value={selectedDevice ? getId(selectedDevice) : ''} onChange={handleChange}>
            {devices
               .find((x) => x.type === 'local')
               ?.devices.map((x, i) => (
                  <MenuItem key={getId(x.device)} value={getId(x.device)}>
                     {x.label || `${defaultName} #${i}`}
                  </MenuItem>
               ))}
            {devices
               .filter((x) => x.type !== 'local')
               .map((x) => x as EquipmentDeviceGroup)
               .map((x, i) => [
                  <ListSubheader key={x.equipmentId}>{x.equipmentName || 'Unnamed device'}</ListSubheader>,
                  x.devices.map((x) => (
                     <MenuItem key={getId(x.device)} value={getId(x.device)}>
                        {x.label || `${defaultName} #${i}`}
                     </MenuItem>
                  )),
               ])}
         </Select>
      </FormControl>
   );
}
