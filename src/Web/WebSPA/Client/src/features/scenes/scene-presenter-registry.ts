import { ScenePresenter } from './types';
import autonmous from './components/autonomous';
import screenShare from './components/screenShare';
import grid from './components/grid';
import breakoutRoom from './components/breakoutRoom';
import activeSpeaker from './components/activeSpeaker';

const presenters: ScenePresenter[] = [autonmous, screenShare, grid, breakoutRoom, activeSpeaker];
export default presenters;
