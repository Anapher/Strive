import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SuccessOrError } from 'src/communication-types';
import { getEquipmentToken } from 'src/core-hub';
import { onInvokeReturn } from 'src/store/signal/actions';
import { ProducerDevice } from 'src/store/webrtc/types';
import { fetchDevices } from './thunks';
import { AnyInputDevice, InputDeviceDto } from './types';

export type PaderConferenceSettings = {
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
         { payload: { source, device } }: PayloadAction<{ source: ProducerDevice; device: AnyInputDevice | undefined }>,
      ) {
         state.obj[source].device = device;
      },
   },
   extraReducers: {
      [getEquipmentToken.returnAction]: (state, action: PayloadAction<SuccessOrError<string>>) => {
         if (action.payload.success) state.equipmentToken = action.payload.response;
      },
      [fetchDevices.fulfilled.type]: (state, { payload }: PayloadAction<InputDeviceDto[]>) => {
         state.availableDevices = payload;
      },
   },
});

export const { closeSettings, openSettings, setAudioGain, setCurrentDevice } = settingsSlice.actions;

export default settingsSlice.reducer;
