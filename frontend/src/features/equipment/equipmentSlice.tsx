import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { fetchDevices } from './thunks';
import { EquipmentDeviceInfo } from './types';

type EquipmentState = {
   availableEquipment: EquipmentDeviceInfo[] | null;
};

const initialState: EquipmentState = {
   availableEquipment: null,
};

const equipmentSlice = createSlice({
   name: 'equipment',
   initialState,
   reducers: {},
   extraReducers: {
      [fetchDevices.fulfilled.type]: (state, { payload }: PayloadAction<EquipmentDeviceInfo[]>) => {
         state.availableEquipment = payload;
      },
   },
});

export default equipmentSlice.reducer;
