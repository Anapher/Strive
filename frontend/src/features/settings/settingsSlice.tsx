import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { _getEquipmentToken } from 'src/core-hub';
import { onInvokeReturn } from 'src/store/signal/actions';

type PaderConferenceSettings = {
   audioGain: number;
};

type SettingsState = {
   open: boolean;
   obj: PaderConferenceSettings;
   equipmentToken: string | null;
};

const initialState: SettingsState = {
   open: false,
   obj: {
      audioGain: 1,
   },
   equipmentToken: null,
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
         state.obj.audioGain = payload;
      },
   },
   extraReducers: {
      [onInvokeReturn(_getEquipmentToken).type]: (state, action: PayloadAction<string>) => {
         state.equipmentToken = action.payload;
      },
   },
});

export const { closeSettings, openSettings, setAudioGain } = settingsSlice.actions;

export default settingsSlice.reducer;
