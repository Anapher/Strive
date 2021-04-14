import { createReducer } from '@reduxjs/toolkit';
import { onConnectionClosed, onConnected, onReconnected, onReconnecting } from './actions';

interface SignalRState {
   isConnected: boolean;
   isReconnecting: boolean;
}

const initialState: SignalRState = {
   isConnected: false,
   isReconnecting: false,
};

export default createReducer(initialState, {
   [onConnected.type]: (state) => {
      state.isConnected = true;
      state.isReconnecting = false;
   },
   [onConnectionClosed.type]: (state) => {
      state.isConnected = false;
      state.isReconnecting = false;
   },
   [onReconnecting.type]: (state) => {
      state.isConnected = false;
      state.isReconnecting = true;
   },
   [onReconnected.type]: (state) => {
      state.isConnected = true;
      state.isReconnecting = false;
   },
});
