import { changeBreakoutRooms, closeBreakoutRooms, openBreakoutRooms } from 'src/core-hub';
import { showErrorOn } from '../notifier/utils';

export default function* mySaga() {
   yield showErrorOn(openBreakoutRooms.returnAction);
   yield showErrorOn(closeBreakoutRooms.returnAction);
   yield showErrorOn(changeBreakoutRooms.returnAction);
}
