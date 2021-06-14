import OpenPollDialog from 'src/features/poll/components/OpenPollDialog';
import usePermission from 'src/hooks/usePermission';
import { POLL_CAN_OPEN } from 'src/permissions';
import allowOverwrite from '../../allow-overwrite-hoc';
import { ScenePresenter, WhiteboardScene } from '../../types';
import RenderWhiteboard from './RenderWhiteboard';
import WhiteboardActionItem from './WhiteboardActionItem';
import WhiteboardListItem from './WhiteboardListItem';

const presenter: ScenePresenter<WhiteboardScene> = {
   type: 'whiteboard',
   AvailableSceneListItem: WhiteboardListItem,
   RenderScene: allowOverwrite(RenderWhiteboard),
   ActionListItem: WhiteboardActionItem,
   AlwaysRender: OpenPollDialog,
   getAutoHideMediaControls: () => false,
   getIsActionListItemVisible: () => usePermission(POLL_CAN_OPEN),
};

export default presenter;
