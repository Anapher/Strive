import BreakoutRoomsDialog from 'src/features/breakout-rooms/components/BreakoutRoomsDialog';
import { ScenePresenter } from '../../types';
import BreakoutRoomListItem from './BreakoutRoomListItem';
import BreakoutRoomScene from './BreakoutRoomScene';
import { OpenBreakoutRoomsItem } from './OpenBreakoutRoomsItem';

const presenter: ScenePresenter = {
   type: 'breakoutRoom',
   ListItem: BreakoutRoomListItem,
   RenderScene: BreakoutRoomScene,
   OpenMenuItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
};

export default presenter;
