import { createPoll, submitPollAnswer, updatePollState, deletePoll } from 'src/core-hub';
import { showErrorOn, showLoadingHubAction } from 'src/store/notifier/utils';

export default function* mySaga() {
   yield showLoadingHubAction(createPoll, 'Loading...');
   yield showLoadingHubAction(updatePollState, 'Loading...');
   yield showLoadingHubAction(deletePoll, 'Loading...');
   yield showErrorOn(submitPollAnswer.returnAction);
}
