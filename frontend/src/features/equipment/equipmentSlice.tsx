import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { events } from 'src/core-hub';
import { EquipmentCommand } from 'src/core-hub.types';
import { onEventOccurred } from 'src/store/signal/actions';

type EquipmentState = {
   command?: EquipmentCommand;
};

const initialState: EquipmentState = {};

const equipmentSlice = createSlice({
   name: 'equipment',
   initialState,
   reducers: {},
   extraReducers: {
      [onEventOccurred(events.onEquipmentCommand).type]: (state, { payload }: PayloadAction<EquipmentCommand>) => {
         state.command = payload;
      },
   },
});

export default equipmentSlice.reducer;
