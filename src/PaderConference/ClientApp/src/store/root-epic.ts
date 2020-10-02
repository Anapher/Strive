import { combineEpics } from 'redux-observable';
import * as authEpics from 'src/features/auth/epics';

export default combineEpics(...Object.values(authEpics));
