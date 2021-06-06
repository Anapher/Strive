import { Size } from 'src/types';

export type ActiveSpeakerScene = {
   type: 'activeSpeaker';
};

export type AutonomousScene = {
   type: 'autonomous';
};

export type BreakoutRoomScene = {
   type: 'breakoutRoom';
};

export type PollScene = {
   type: 'poll';
   pollId: string;
};

export type GridScene = {
   type: 'grid';
};

export type PresenterScene = {
   type: 'presenter';
   presenterParticipantId: string;
};

export type ScreenShareScene = {
   type: 'screenShare';
   participantId: string;
};

export type TalkingStickScene = {
   type: 'talkingStick';
   mode: TalkingStickMode;
};

export type TalkingStickMode = 'race' | 'queue' | 'moderated' | 'speakerPassStick';

export type Scene =
   | ActiveSpeakerScene
   | AutonomousScene
   | BreakoutRoomScene
   | PollScene
   | GridScene
   | PresenterScene
   | ScreenShareScene
   | TalkingStickScene;

export type SynchronizedScene = {
   selectedScene: Scene;
   overwrittenContent?: Scene | null;
   availableScenes: Scene[];
   sceneStack: Scene[];
};

export type SynchronizedTalkingStick = {
   currentSpeakerId?: string;
   speakerQueue: string[];
};

export type ModeSceneListItemProps = {
   availableScene?: Scene;
   selectedScene: Scene;
   onChangeScene: (newScene: Scene) => void;
};

export type AvailableSceneListItemProps<S extends Scene = Scene> = {
   scene: S;
   stack: Scene[];

   onChangeScene: (newScene: Scene) => void;
};

export type ActionListItemProps = {
   onClose: () => void;
};

export type RenderSceneProps<S extends Scene = Scene> = {
   className?: string;
   dimensions: Size;
   scene: S;

   next: (additionalProps?: any) => React.ReactNode | null;
};

export type ActiveDescriptorProps<S extends Scene = Scene> = {
   scene: S;
};

export type ScenePresenter<S extends Scene> = {
   type: S['type'];

   getSceneId?: (scene: S) => string;

   ActiveDescriptor?: React.ComponentType<ActiveDescriptorProps<S>>;

   ModeSceneListItem?: React.ComponentType<ModeSceneListItemProps>;
   AvailableSceneListItem?: React.ComponentType<AvailableSceneListItemProps<S>>;
   ActionListItem?: React.ComponentType<ActionListItemProps>;

   AlwaysRender?: React.ComponentType;

   RenderScene: React.ComponentType<RenderSceneProps<S>>;

   getAutoHideMediaControls?: (scene: S, participantId: string) => boolean | undefined;
   getIsActionListItemVisible?: () => void;
};

export type ActiveParticipantData = {
   /** if the participant is not currently active, this property contains the exact timestamp the participant went inactive */
   deletedOn?: string;

   /** a number that determines the order of this participant */
   orderNumber: number;
};

export type ActiveParticipants = {
   [participantId: string]: ActiveParticipantData;
};
