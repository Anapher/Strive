import { spawn } from 'redux-saga/effects';
import scenes from 'src/features/scenes/sagas';
import media from 'src/features/media/sagas';
import settings from 'src/features/settings/sagas';
import conference from 'src/features/conference/sagas';
import chat from 'src/features/chat/sagas';
import breakoutRooms from 'src/features/breakout-rooms/sagas';
import createConference from 'src/features/create-conference/sagas';
import equipment from 'src/features/equipment/sagas';

export default function* rootSaga() {
   yield spawn(scenes);
   yield spawn(media);
   yield spawn(settings);
   yield spawn(conference);
   yield spawn(chat);
   yield spawn(breakoutRooms);
   yield spawn(createConference);
   yield spawn(equipment);
}
