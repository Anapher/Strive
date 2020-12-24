import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type DiagnosticsState = {
   open: boolean;
};

const initialState: DiagnosticsState = {
   open: false,
};

const debugSlice = createSlice({
   name: 'diagnostics',
   initialState,
   reducers: {
      setOpen(state, { payload }: PayloadAction<boolean>) {
         state.open = payload;
      },
   },
});

export const { setOpen } = debugSlice.actions;

export default debugSlice.reducer;
