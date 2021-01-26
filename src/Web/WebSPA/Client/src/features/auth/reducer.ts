import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type AuthState = {
   participantId: string | null;
};

const initialState: AuthState = {
   participantId: null,
};

const authSlice = createSlice({
   name: 'auth',
   initialState,
   reducers: {
      setParticipantId(state, { payload }: PayloadAction<string | null>) {
         state.participantId = payload;
      },
   },
});

export const { setParticipantId } = authSlice.actions;

export default authSlice.reducer;
