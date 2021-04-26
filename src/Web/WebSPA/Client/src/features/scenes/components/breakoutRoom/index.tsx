import BreakoutRoomsDialog from 'src/features/breakout-rooms/components/BreakoutRoomsDialog';
import { ScenePresenter } from '../../types';
import BreakoutRoomListItem from './BreakoutRoomListItem';
import BreakoutRoomScene from './BreakoutRoomScene';
import { OpenBreakoutRoomsItem } from './OpenBreakoutRoomsItem';

const presenter: ScenePresenter = {
   type: 'breakoutRoom',
   AvailableSceneListItem: BreakoutRoomListItem,
   RenderScene: BreakoutRoomScene,
   ActionListItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
};

export default presenter;
