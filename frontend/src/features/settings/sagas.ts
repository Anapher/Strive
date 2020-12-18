import { PayloadAction } from '@reduxjs/toolkit';
import { put, select, takeEvery } from 'redux-saga/effects';
import { events } from 'src/core-hub';
import { RootState } from 'src/store';
import { onEventOccurred } from 'src/store/signal/actions';
import { ProducerDevices } from 'src/store/webrtc/types';
import { ConnectedEquipmentDto } from '../media/types';
import { showMessage } from '../notifier/actions';
import { PaderConferenceSettings, setCurrentDevice } from './reducer';
import { fetchDevices } from './thunks';
import { InputDeviceDto } from './types';

/**
 * Executed when local devices changed. Check if the selected devices are still available
 */
function* updateLocalDevices({ payload }: PayloadAction<InputDeviceDto[]>) {
   const deviceSettings: PaderConferenceSettings = yield select((state: RootState) => state.settings.obj);

   for (const producerDevice of ProducerDevices) {
      const device = deviceSettings[producerDevice].device;
      if (!device) continue;

      if (device.type === 'local') {
         if (!payload.find((x) => x.deviceId === device.deviceId)) {
            yield put(setCurrentDevice({ source: producerDevice, device: undefined }));

            yield put(
               showMessage({
                  variant: 'warning',
                  message: `The local device for source ${producerDevice} was disconnected, fall back to default device.`,
               }),
            );
         }
      }
   }
}

/**
 * Executed when equipment changed. Check if the selected devices are still available
 */
function* updateEquipment({ payload }: PayloadAction<ConnectedEquipmentDto[]>) {
   const deviceSettings: PaderConferenceSettings = yield select((state: RootState) => state.settings.obj);

   for (const producerDevice of ProducerDevices) {
      const device = deviceSettings[producerDevice].device;
      if (!device) continue;

      if (device.type === 'equipment') {
         if (
            !payload
               .find((x) => x.equipmentId === device.equipmentId)
               ?.devices?.find((x) => x.deviceId === device.deviceId)
         ) {
            yield put(setCurrentDevice({ source: producerDevice, device: undefined }));

            yield put(
               showMessage({
                  variant: 'warning',
                  message: `The equipment device for source ${producerDevice} was disconnected, fall back to default device.`,
               }),
            );
         }
      }
   }
}

function* mySaga() {
   yield takeEvery(fetchDevices.fulfilled, updateLocalDevices);
   yield takeEvery(onEventOccurred(events.onEquipmentUpdated).type, updateEquipment);
}

export default mySaga;
