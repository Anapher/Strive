import { DEFAULT_PREFIX, SIGNALR_CONNECTED, SIGNALR_DISCONNECTED } from './action-types';

interface SignalRState {
   isConnected: boolean;
}

const initialState: SignalRState = {
   isConnected: false,
};

export default function(prefix: string = DEFAULT_PREFIX) {
   return (state = initialState, action: any): SignalRState => {
      switch (action.type) {
         case `${prefix}::${SIGNALR_CONNECTED}`:
            return { ...state, isConnected: true };
         case `${prefix}::${SIGNALR_DISCONNECTED}`:
            return { ...state, isConnected: false };
         default:
            return state;
      }
   };
}
