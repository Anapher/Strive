import { sendChatMessage } from 'src/core-hub';
import { showErrorOn } from 'src/store/notifier/utils';

export default function* mySaga() {
   yield showErrorOn(sendChatMessage.returnAction);
}
