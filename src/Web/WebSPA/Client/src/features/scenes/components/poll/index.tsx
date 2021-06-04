import OpenPollDialog from 'src/features/poll/components/OpenPollDialog';
import usePermission from 'src/hooks/usePermission';
import { POLL_CAN_OPEN } from 'src/permissions';
import { PollScene, ScenePresenter } from '../../types';
import PollListItem from './PollListItem';
import PollActionItem from './PollActionItem';
import RenderPollScene from './RenderPollScene';
import allowOverwrite from '../../allow-overwrite-hoc';

const presenter: ScenePresenter<PollScene> = {
   type: 'poll',
   AvailableSceneListItem: PollListItem,
   RenderScene: allowOverwrite(RenderPollScene),
   ActionListItem: PollActionItem,
   AlwaysRender: OpenPollDialog,
   getAutoHideMediaControls: () => false,
   getIsActionListItemVisible: () => usePermission(POLL_CAN_OPEN),
};

export default presenter;
