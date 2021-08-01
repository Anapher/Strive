import BreakoutRoomsDialog from 'src/features/breakout-rooms/components/BreakoutRoomsDialog';
import usePermission from 'src/hooks/usePermission';
import { ROOMS_CAN_CREATE_REMOVE } from 'src/permissions';
import allowOverwrite from '../../allow-overwrite-hoc';
import { BreakoutRoomScene, ScenePresenter } from '../../types';
import BreakoutRoomListItem from './BreakoutRoomListItem';
import OpenBreakoutRoomsItem from './OpenBreakoutRoomsItem';
import RenderBreakoutRoom from './RenderBreakoutRoom';

const presenter: ScenePresenter<BreakoutRoomScene> = {
   type: 'breakoutRoom',
   AvailableSceneListItem: BreakoutRoomListItem,
   RenderScene: allowOverwrite(RenderBreakoutRoom),
   ActionListItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
   getAutoHideMediaControls: () => false,
   getIsActionListItemVisible: () => usePermission(ROOMS_CAN_CREATE_REMOVE),
};

export default presenter;
