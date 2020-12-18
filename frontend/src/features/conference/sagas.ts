import { closeConference, openConference } from 'src/core-hub';
import { showErrorOn } from '../notifier/utils';

export default function* mySaga() {
   yield showErrorOn(openConference.returnAction);
   yield showErrorOn(closeConference.returnAction);
}
