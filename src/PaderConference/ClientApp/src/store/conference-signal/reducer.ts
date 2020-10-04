import { createReducer } from '@reduxjs/toolkit';
import { onConferenceConnectionClosed, onConferenceJoined } from './actions';

interface SignalRState {
   isConnected: boolean;
}

const initialState: SignalRState = {
   isConnected: false,
};

export default createReducer(initialState, {
   [onConferenceJoined.type]: (state) => {
      state.isConnected = true;
   },
   [onConferenceConnectionClosed.type]: (state) => {
      state.isConnected = false;
   },
});
