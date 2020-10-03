import { DEFAULT_PREFIX, ON_CONFERENCE_JOINED, ON_CONFERENCE_CONNECTION_CLOSED } from './action-types';

interface SignalRState {
   isConnected: boolean;
}

const initialState: SignalRState = {
   isConnected: false,
};

export default function (prefix: string = DEFAULT_PREFIX) {
   return (state = initialState, action: any): SignalRState => {
      switch (action.type) {
         case `${prefix}::${ON_CONFERENCE_JOINED}`:
            return { ...state, isConnected: true };
         case `${prefix}::${ON_CONFERENCE_CONNECTION_CLOSED}`:
            return { ...state, isConnected: false };
         default:
            return state;
      }
   };
}
