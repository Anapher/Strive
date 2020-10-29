import { FormControl, InputLabel, ListSubheader, makeStyles, MenuItem, Select } from '@material-ui/core';
import _ from 'lodash';
import React, { Fragment } from 'react';
import { DeviceGroup, EquipmentDeviceGroup } from '../selectors';
import { AnyInputDevice } from '../types';
import clsx from 'classnames';

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

   selectedDeviceId?: string;
};

export default function DeviceSelector({ devices, defaultName, className, selectedDeviceId, onChange, label }: Props) {
   const selectId = defaultName.toLowerCase() + '-select';
   const classes = useStyles();

   const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
      const deviceId = event.target.value as string;
      const viewModel = _(devices)
         .flatMap((x) => x.devices)
         .find((x) => x.device.deviceId === deviceId);

      if (viewModel) {
         onChange(viewModel.device);
      }
   };

   return (
      <FormControl className={clsx(className, classes.control)}>
         <InputLabel htmlFor={selectId}>{label}</InputLabel>
         <Select id={selectId} value={selectedDeviceId ?? ''} onChange={handleChange}>
            {devices
               .find((x) => x.type === 'local')
               ?.devices.map((x, i) => (
                  <MenuItem key={x.device.deviceId} value={x.device.deviceId}>
                     {x.label || `${defaultName} #${i}`}
                  </MenuItem>
               ))}
            {devices
               .filter((x) => x.type !== 'local')
               .map((x) => x as EquipmentDeviceGroup)
               .map((x, i) => (
                  <Fragment key={x.equipmentId}>
                     <ListSubheader>{x.equipmentName || 'Unnamed device'}</ListSubheader>
                     {x.devices.map((x) => (
                        <MenuItem key={x.device.deviceId}>{x.label || `${defaultName} #${i}`}</MenuItem>
                     ))}
                  </Fragment>
               ))}
         </Select>
      </FormControl>
   );
}
