import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { _getEquipmentToken } from 'src/core-hub';
import { onInvokeReturn } from 'src/store/signal/actions';
import { ProducerSource } from 'src/store/webrtc/types';
import { fetchDevices } from './thunks';
import { AnyInputDevice, InputDeviceDto } from './types';

type PaderConferenceSettings = {
   mic: {
      device?: AnyInputDevice;
      audioGain: number;
   };
   webcam: {
      device?: AnyInputDevice;
   };
   screen: {
      device?: AnyInputDevice;
   };
};

type SettingsState = {
   open: boolean;
   obj: PaderConferenceSettings;
   equipmentToken: string | null;
   availableDevices: InputDeviceDto[] | null;
};

const initialState: SettingsState = {
   open: false,
   obj: {
      mic: {
         audioGain: 1,
      },
      webcam: {},
      screen: {},
   },
   equipmentToken: null,
   availableDevices: null,
};

const settingsSlice = createSlice({
   name: 'settings',
   initialState,
   reducers: {
      openSettings(state) {
         state.open = true;
      },
      closeSettings(state) {
         state.open = false;
      },
      setAudioGain(state, { payload }: PayloadAction<number>) {
         state.obj.mic.audioGain = payload;
      },
      setCurrentDevice(
         state,
         { payload: { source, device } }: PayloadAction<{ source: ProducerSource; device: AnyInputDevice | undefined }>,
      ) {
         state.obj[source].device = device;
      },
   },
   extraReducers: {
      [onInvokeReturn(_getEquipmentToken).type]: (state, action: PayloadAction<string>) => {
         state.equipmentToken = action.payload;
      },
      [fetchDevices.fulfilled.type]: (state, { payload }: PayloadAction<InputDeviceDto[]>) => {
         state.availableDevices = payload;
      },
   },
});

export const { closeSettings, openSettings, setAudioGain, setCurrentDevice } = settingsSlice.actions;

export default settingsSlice.reducer;
