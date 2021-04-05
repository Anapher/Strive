import { Size } from 'src/types';

export type GridScene = {
   type: 'grid';
};

export type ActiveSpeakerScene = {
   type: 'activeSpeaker';
};

export type ScreenShareScene = {
   type: 'screenShare';
   participantId: string;
};

export type BreakoutRoomScene = {
   type: 'breakoutRoom';
};

export type AutonomousScene = {
   type: 'autonomous';
};

export type FollowServer = {
   type: 'followServer';
};

export type ViewableScene = GridScene | ActiveSpeakerScene | BreakoutRoomScene | ScreenShareScene;
export type Scene = ViewableScene | AutonomousScene;

export type SceneConfig = {
   hideParticipantsWithoutWebcam?: boolean;
};

export type ActiveScene = {
   isControlled: boolean;
   scene?: Scene;
   config: SceneConfig;
};

export type ConfiguredScene = {
   scene?: Scene;
   config: SceneConfig;
};

export type SynchronizedScene = {
   availableScenes: Scene[];
   active: ActiveScene;
};

export type SceneViewModel = {
   scene: Scene;
   isApplied: boolean;
   isCurrent: boolean;
   id: string;
};

export type SceneListItemProps = {
   scene: Scene;
   applied: boolean;
   current: boolean;

   onChangeScene: (newScene: Scene) => void;
};

export type ActiveSceneMenuItemProps = {
   onClose: () => void;
};

export type RenderSceneProps = {
   className?: string;
   dimensions: Size;
   scene: Scene;
   setShowWebcamUnderChat: (show: boolean) => void;
   setAutoHideControls: (autoHide: boolean) => void;
};

export type ScenePresenter = {
   type: Scene['type'];

   ListItem: React.ComponentType<SceneListItemProps>;
   AlwaysRender?: React.ComponentType;
   OpenMenuItem?: React.ComponentType<ActiveSceneMenuItemProps>;

   RenderScene: React.ComponentType<RenderSceneProps>;
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
