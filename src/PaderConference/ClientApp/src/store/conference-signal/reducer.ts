import { createReducer } from '@reduxjs/toolkit';
import {
   onConferenceConnectionClosed,
   onConferenceJoined,
   onConferenceReconnected,
   onConferenceReconnecting,
} from './actions';

interface SignalRState {
   isConnected: boolean;
   isReconnecting: boolean;
}

const initialState: SignalRState = {
   isConnected: false,
   isReconnecting: false,
};

export default createReducer(initialState, {
   [onConferenceJoined.type]: (state) => {
      state.isConnected = true;
      state.isReconnecting = false;
   },
   [onConferenceConnectionClosed.type]: (state) => {
      state.isConnected = false;
   },
   [onConferenceReconnecting.type]: (state) => {
      state.isConnected = false;
      state.isReconnecting = true;
   },
   [onConferenceReconnected.type]: (state) => {
      state.isConnected = true;
      state.isReconnecting = false;
   },
});
