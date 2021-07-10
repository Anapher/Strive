import { createSlice } from '@reduxjs/toolkit';
import { WHITEBOARDS } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { SynchronizedWhiteboards } from './types';

export type WhiteboardState = {
   whiteboards: SynchronizedWhiteboards | null;
};

const initialState: WhiteboardState = {
   whiteboards: null,
};

const whiteboardSlice = createSlice({
   name: 'whiteboard',
   initialState,
   reducers: {},
   extraReducers: {
      ...synchronizeObjectState([{ type: 'single', baseId: WHITEBOARDS, propertyName: 'whiteboards' }]),
   },
});

export default whiteboardSlice.reducer;
