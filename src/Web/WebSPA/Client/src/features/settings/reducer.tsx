import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import { getEquipmentToken } from 'src/core-hub';
import { ProducerDevice } from 'src/store/webrtc/types';
import { fetchDevices } from './thunks';
import { AnyInputDevice, InputDeviceDto } from './types';

export type StriveSettings = {
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
   diagnostics: {
      enableVideoOverlay: boolean;
   };
};

type SettingsState = {
   open: boolean;
   obj: StriveSettings;
   equipmentToken: string | null;
   equipmentTokenError: DomainError | null;
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
      diagnostics: {
         enableVideoOverlay: false,
      },
   },
   equipmentToken: null,
   equipmentTokenError: null,
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
      setEnableVideoOverlay(state, { payload }: PayloadAction<boolean>) {
         state.obj.diagnostics.enableVideoOverlay = payload;
      },
   },
   extraReducers: {
      [getEquipmentToken.returnAction]: (state, action: PayloadAction<SuccessOrError<string>>) => {
         if (action.payload.success) {
            state.equipmentToken = action.payload.response;
         } else {
            state.equipmentTokenError = action.payload.error;
         }
      },
      [getEquipmentToken.action]: (state) => {
         state.equipmentTokenError = null;
      },
      [fetchDevices.fulfilled.type]: (state, { payload }: PayloadAction<InputDeviceDto[]>) => {
         state.availableDevices = payload;
      },
   },
});

export const {
   closeSettings,
   openSettings,
   setAudioGain,
   setCurrentDevice,
   setEnableVideoOverlay,
} = settingsSlice.actions;

export default settingsSlice.reducer;
