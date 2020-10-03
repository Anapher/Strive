import { combineEpics } from 'redux-observable';
import * as authEpics from 'src/features/auth/epics';
import * as conferenceEpics from 'src/features/conference/epics';

export default combineEpics(...Object.values(authEpics), ...Object.values(conferenceEpics));
