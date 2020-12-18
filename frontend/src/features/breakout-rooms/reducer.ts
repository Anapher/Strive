import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { BreakoutRoomsInfo } from './types';

export type BreakoutRoomsState = {
   synchronized: BreakoutRoomsInfo | null;
   creationDialogOpen: boolean;
};

const initialState: BreakoutRoomsState = {
   synchronized: null,
   creationDialogOpen: false,
};

const breakoutRoomsSlice = createSlice({
   name: 'conference',
   initialState,
   reducers: {
      setCreationDialogOpen(state, { payload }: PayloadAction<boolean>) {
         state.creationDialogOpen = payload;
      },
   },
   extraReducers: {
      ...createSynchronizeObjectReducer({ name: 'breakoutRooms', stateName: 'synchronized' }),
   },
});

export const { setCreationDialogOpen } = breakoutRoomsSlice.actions;

export default breakoutRoomsSlice.reducer;
