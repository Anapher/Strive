import { createSlice } from '@reduxjs/toolkit';
import { ROOMS, SynchronizedRooms } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';

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
      ...synchronizeObjectState({ type: 'exactId', syncObjId: ROOMS, propertyName: 'synchronized' }),
   },
});

export default roomsSlice.reducer;
