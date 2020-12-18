import { createRooms, removeRooms, switchRoom } from 'src/core-hub';
import { showErrorOn } from '../notifier/utils';

export default function* mySaga() {
   yield showErrorOn(createRooms.returnAction);
   yield showErrorOn(removeRooms.returnAction);
   yield showErrorOn(switchRoom.returnAction);
}
