import { useSelector } from 'react-redux';
import { ActiveSceneInfo } from '../scenes/types';
import ActiveMenuItem from './components/ActiveMenuItem';
import BreakoutRoomsDialog from './components/BreakoutRoomsDialog';
import { OpenBreakoutRoomsItem } from './components/OpenBreakoutRoomsItem';
import { selectIsBreakoutRoomsOpen } from './selectors';

const breakoutRooms: ActiveSceneInfo = {
   useIsActive: () => useSelector(selectIsBreakoutRoomsOpen),
   ActiveMenuItem: ActiveMenuItem,
   OpenMenuItem: OpenBreakoutRoomsItem,
   AlwaysRender: BreakoutRoomsDialog,
};

export default breakoutRooms;
