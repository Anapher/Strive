import * as coreHub from 'src/core-hub';
import { closeConference, openConference } from 'src/core-hub';
import { showErrorOn, showLoadingHubAction } from 'src/store/notifier/utils';

export default function* mySaga() {
   yield showErrorOn(openConference.returnAction);
   yield showErrorOn(closeConference.returnAction);
   yield showLoadingHubAction(coreHub.fetchPermissions, 'Fetch permissions...');
}
