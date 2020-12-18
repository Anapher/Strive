import { sendChatMessage } from 'src/core-hub';
import { showErrorOn } from '../notifier/utils';

export default function* mySaga() {
   yield showErrorOn(sendChatMessage.returnAction);
}
