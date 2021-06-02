import { createPoll } from 'src/core-hub';
import { showLoadingHubAction } from 'src/store/notifier/utils';

export default function* mySaga() {
   yield showLoadingHubAction(createPoll, 'Loading...');
}
