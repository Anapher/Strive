import BreakoutRoomsDialog from 'src/features/breakout-rooms/components/BreakoutRoomsDialog';
import usePermission from 'src/hooks/usePermission';
import { ROOMS_CAN_CREATE_REMOVE } from 'src/permissions';
import { BreakoutRoomScene, ScenePresenter } from '../../types';
import BreakoutRoomListItem from './BreakoutRoomListItem';
import OpenBreakoutRoomsItem from './OpenBreakoutRoomsItem';
import RenderBreakoutRoom from './RenderBreakoutRoom';

const presenter: ScenePresenter<BreakoutRoomScene> = {
   type: 'breakoutRoom',
   AvailableSceneListItem: BreakoutRoomListItem,
   RenderScene: RenderBreakoutRoom,
   ActionListItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
   getAutoHideMediaControls: () => false,
   getIsActionListItemVisible: () => usePermission(ROOMS_CAN_CREATE_REMOVE),
};

export default presenter;
