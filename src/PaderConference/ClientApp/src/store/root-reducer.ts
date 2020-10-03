import { combineReducers } from 'redux';

import auth from '../features/auth/reducer';
import conference from '../features/conference/reducer';
import signalrReducer from './conference-signal/reducer';

const rootReducer = combineReducers({
   auth,
   conference,
   signalr: signalrReducer(),
});

export default rootReducer;
