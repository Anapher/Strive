import { combineReducers } from 'redux';

import auth from '../features/auth/authSlice';
import signIn from '../features/auth/signInReducer';
import conference from '../features/conference/conferenceSlice';
import chat from '../features/chat/chatSlice';
import createConference from '../features/create-conference/createConferenceSlice';
import signalr from './conference-signal/reducer';
import media from '../features/media/mediaSlice';
import rooms from '../features/rooms/roomsSlice';
import notifier from '../features/notifier/notifierSlice';

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
});

export default rootReducer;
