import activeSpeaker from './components/activeSpeaker';
import autonomous from './components/autonomous';
import breakoutRoom from './components/breakoutRoom';
import grid from './components/grid';
import presenter from './components/presenter';
import screenShare from './components/screenShare';
import talkingStick from './components/talkingStick';
import whiteboard from './components/whiteboard';
import poll from './components/poll';
import { ScenePresenter } from './types';

const presenters: ScenePresenter<any>[] = [
   autonomous,
   screenShare,
   grid,
   breakoutRoom,
   activeSpeaker,
   talkingStick,
   presenter,
   poll,
   whiteboard,
];

export default presenters;
