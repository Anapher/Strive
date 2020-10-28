import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { _getEquipmentToken } from 'src/core-hub';
import { onInvokeReturn } from 'src/store/signal/actions';

type EquipmentState = {
   token: string | null;
};

const initialState: EquipmentState = {
   token: null,
};

const equipmentSlice = createSlice({
   name: 'equipment',
   initialState,
   reducers: {},
   extraReducers: {
      [onInvokeReturn(_getEquipmentToken).type]: (state, action: PayloadAction<string>) => {
         state.token = action.payload;
      },
   },
});

export default equipmentSlice.reducer;
