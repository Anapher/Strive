import { Box, Button, FormControl, InputLabel, ListSubheader, makeStyles, MenuItem, Select } from '@material-ui/core';
import clsx from 'classnames';
import _, { Collection } from 'lodash';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { showMessage } from 'src/store/notifier/actions';
import { DeviceGroup, EquipmentDeviceGroup } from '../selectors';
import { fetchDevices } from '../thunks';
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
   device.type === 'local' ? `local/${device.deviceId}` : `${device.connectionId}/${device.deviceId}`;

export default function DeviceSelector({ devices, defaultName, className, selectedDevice, onChange, label }: Props) {
   const selectId = defaultName.toLowerCase() + '-select';
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
      const value = event.target.value as string;
      if (!value) return;

      let deviceId: string | undefined;
      let possibleDevices: Collection<DeviceGroup> | undefined;

      if (value.startsWith('local/')) {
         possibleDevices = _(devices).filter((x) => x.type === 'local');
         deviceId = value.split('/')[1];
      } else {
         const split = value.split('/');
         possibleDevices = _(devices).filter((x) => x.type === 'equipment' && x.connectionId === split[0]);
         deviceId = split[1];
      }

      const viewModel = possibleDevices.flatMap((x) => x.devices).find((x) => x.device.deviceId === deviceId);

      if (viewModel) {
         onChange(viewModel.device);
      }
   };

   const handleRefresh = () => {
      dispatch(
         showMessage({
            type: 'action',
            message: t('conference.notifications.fetch_devices.loading'),
            failedOn: { type: fetchDevices.rejected.type, message: t('conference.notifications.fetch_devices.error') },
            succeededOn: {
               type: fetchDevices.fulfilled.type,
               message: t('conference.notifications.fetch_devices.success'),
            },
         }),
      );
      dispatch(fetchDevices());
   };

   const defaultDevice = devices[0]?.devices[0]?.device;

   return (
      <Box display="flex" alignItems="flex-end">
         <FormControl className={clsx(className, classes.control)}>
            <InputLabel htmlFor={selectId}>{label}</InputLabel>
            <Select
               id={selectId}
               value={selectedDevice ? getId(selectedDevice) : defaultDevice ? getId(defaultDevice) : ''}
               onChange={handleChange}
            >
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
                     <ListSubheader key={x.connectionId}>
                        {x.equipmentName || t('conference.settings.unnamed_device')}
                     </ListSubheader>,
                     x.devices.map((x) => (
                        <MenuItem key={getId(x.device)} value={getId(x.device)}>
                           {x.label || `${defaultName} #${i}`}
                        </MenuItem>
                     )),
                  ])}
            </Select>
         </FormControl>
         <Box ml={1} onClick={handleRefresh}>
            <Button>{t('common:refresh')}</Button>
         </Box>
      </Box>
   );
}
