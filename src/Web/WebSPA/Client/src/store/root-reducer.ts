import { combineReducers } from 'redux';
import { PersistConfig, persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import auth from '../features/auth/reducer';
import breakoutRooms from '../features/breakout-rooms/reducer';
import chat from '../features/chat/reducer';
import conference from '../features/conference/reducer';
import createConference from '../features/create-conference/reducer';
import diagnostics from '../features/diagnostics/reducer';
import equipment from '../features/equipment/reducer';
import media from '../features/media/reducer';
import poll from '../features/poll/reducer';
import rooms from '../features/rooms/reducer';
import scenes from '../features/scenes/reducer';
import settings from '../features/settings/reducer';
import whiteboard from '../features/whiteboard/reducer';
import signalr from './signal/reducer';

const settingsPersistConfig: PersistConfig<any> = {
   key: 'settings',
   storage: storage,
   whitelist: ['obj'],
};

const rootReducer = combineReducers({
   auth,
   conference,
   createConference,
   chat,
   signalr,
   media,
   rooms,
   settings: persistReducer(settingsPersistConfig, settings),
   equipment,
   scenes,
   breakoutRooms,
   diagnostics,
   poll,
   whiteboard,
});

export default rootReducer;
