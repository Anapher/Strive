import { combineReducers } from 'redux';
import conference from '../features/conference/reducer';
import chat from '../features/chat/reducer';
import createConference from '../features/create-conference/reducer';
import signalr from './signal/reducer';
import media from '../features/media/reducer';
import rooms from '../features/rooms/reducer';
import settings from '../features/settings/reducer';
import equipment from '../features/equipment/reducer';
import scenes from '../features/scenes/reducer';
import breakoutRooms from '../features/breakout-rooms/reducer';
import diagnostics from '../features/diagnostics/reducer';
import auth from '../features/auth/reducer';
import storage from 'redux-persist/lib/storage';
import { PersistConfig, persistReducer } from 'redux-persist';

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
});

export default rootReducer;
