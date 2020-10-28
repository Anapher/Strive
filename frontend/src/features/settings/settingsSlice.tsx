import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type PaderConferenceSettings = {
   audioGain: number;
};

type SettingsState = {
   open: boolean;
   obj: PaderConferenceSettings;
};

const initialState: SettingsState = {
   open: false,
   obj: {
      audioGain: 1,
   },
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
});

export const { closeSettings, openSettings, setAudioGain } = settingsSlice.actions;

export default settingsSlice.reducer;
