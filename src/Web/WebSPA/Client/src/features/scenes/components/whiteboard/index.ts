import usePermission from 'src/hooks/usePermission';
import { WHITEBOARD_CAN_CREATE } from 'src/permissions';
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
   getAutoHideMediaControls: () => false,
   getIsActionListItemVisible: () => usePermission(WHITEBOARD_CAN_CREATE),
};

export default presenter;
