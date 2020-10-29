import { createAsyncThunk } from '@reduxjs/toolkit';
import { InputDeviceDto } from './types';

export const fetchDevices = createAsyncThunk('equipment/fetchDevices', async () => {
   const devices = await navigator.mediaDevices.enumerateDevices();

   const result: InputDeviceDto[] = [];
   for (const device of devices) {
      switch (device.kind) {
         case 'audioinput':
            result.push({ deviceId: device.deviceId, label: device.label, source: 'mic' });
            break;
         case 'videoinput':
            result.push({ deviceId: device.deviceId, label: device.label, source: 'webcam' });
            break;
         default:
            break;
      }
   }

   if (typeof (navigator.mediaDevices as any).getDisplayMedia === 'function') {
      result.push({ deviceId: 'getDisplayMedia', label: 'Screen', source: 'screen' });
   }

   return result;
});
