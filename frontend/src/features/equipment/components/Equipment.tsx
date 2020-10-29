import { Typography } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { fetchDevices } from '../thunks';
import { getDeviceName } from '../ua-utils';
import AvailableEquipmentTable from './AvailableEquipmentTable';
import * as coreHub from 'src/core-hub';

const deviceName = getDeviceName();

export default function Equipment() {
   const dispatch = useDispatch();
   const availableEquipment = useSelector((state: RootState) => state.equipment.availableEquipment);

   useEffect(() => {
      dispatch(fetchDevices());
   }, []);

   useEffect(() => {
      if (availableEquipment) {
         dispatch(coreHub.registerEquipment({ name: deviceName, devices: availableEquipment }));
      }
   }, [availableEquipment]);

   return (
      <div>
         <Typography variant="h4">{deviceName}</Typography>
         <AvailableEquipmentTable />
      </div>
   );
}
