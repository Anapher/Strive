import BreakoutRoomsDialog from 'src/features/breakout-rooms/components/BreakoutRoomsDialog';
import { BreakoutRoomScene, ScenePresenter } from '../../types';
import BreakoutRoomListItem from './BreakoutRoomListItem';
import { OpenBreakoutRoomsItem } from './OpenBreakoutRoomsItem';
import RenderBreakoutRoom from './RenderBreakoutRoom';

const presenter: ScenePresenter<BreakoutRoomScene> = {
   type: 'breakoutRoom',
   AvailableSceneListItem: BreakoutRoomListItem,
   RenderScene: RenderBreakoutRoom,
   ActionListItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
   getAutoHideMediaControls: () => false,
};

export default presenter;
