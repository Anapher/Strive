import { ScenePresenter } from './types';
import autonomous from './components/autonomous';
import screenShare from './components/screenShare';
import grid from './components/grid';
import breakoutRoom from './components/breakoutRoom';
import activeSpeaker from './components/activeSpeaker';
import talkingStick from './components/talkingStick';
import presenter from './components/presenter';

const presenters: ScenePresenter[] = [
   autonomous,
   screenShare,
   grid,
   breakoutRoom,
   activeSpeaker,
   talkingStick,
   presenter,
];

export default presenters;
