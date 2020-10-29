import { createSlice } from '@reduxjs/toolkit';

type EquipmentState = {
   test?: string;
};

const initialState: EquipmentState = {};

const equipmentSlice = createSlice({
   name: 'equipment',
   initialState,
   reducers: {},
   extraReducers: {},
});

export default equipmentSlice.reducer;
