import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { _getEquipmentToken } from 'src/core-hub';
import { onInvokeReturn } from 'src/store/signal/actions';
import { fetchDevices } from './thunks';
import { AnyInputDevice, InputDeviceDto } from './types';

type PaderConferenceSettings = {
   audio: {
      device?: AnyInputDevice;
      audioGain: number;
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
      audio: {
         audioGain: 1,
      },
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
         state.obj.audio.audioGain = payload;
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

export const { closeSettings, openSettings, setAudioGain } = settingsSlice.actions;

export default settingsSlice.reducer;
