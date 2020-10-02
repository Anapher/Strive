import { combineReducers } from 'redux';

import auth from '../features/auth/reducer';
import signalrReducer from './signalr/signalr-reducer';

const rootReducer = combineReducers({
   auth,
   signalr: signalrReducer(),
});

export default rootReducer;
