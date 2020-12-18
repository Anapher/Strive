import { createSlice } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { SynchronizedRooms } from './types';

export type RoomsState = {
   synchronized: SynchronizedRooms | null;
};

const initialState: RoomsState = {
   synchronized: null,
};

const roomsSlice = createSlice({
   name: 'rooms',
   initialState,
   reducers: {},
   extraReducers: {
      ...createSynchronizeObjectReducer({ name: 'rooms', stateName: 'synchronized' }),
   },
});

export default roomsSlice.reducer;
