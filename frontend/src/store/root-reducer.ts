import { combineReducers } from 'redux';

import auth from '../features/auth/reducer';
import signIn from '../features/auth/sign-in-reducer';
import conference from '../features/conference/reducer';
import chat from '../features/chat/reducer';
import createConference from '../features/create-conference/reducer';
import signalr from './signal/reducer';
import media from '../features/media/reducer';
import rooms from '../features/rooms/reducer';
import notifier from '../features/notifier/reducer';
import settings from '../features/settings/reducer';
import equipment from '../features/equipment/reducer';
import scenes from '../features/scenes/reducer';
import breakoutRooms from '../features/breakout-rooms/reducer';

const rootReducer = combineReducers({
   auth,
   signIn,
   conference,
   createConference,
   chat,
   signalr,
   media,
   rooms,
   notifier,
   settings,
   equipment,
   scenes,
   breakoutRooms,
});

export default rootReducer;
