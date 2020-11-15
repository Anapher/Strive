export type GridScene = {
   type: 'grid';
   hideParticipantsWithoutWebcam?: boolean;
};

export type ScreenShareScene = {
   type: 'screenshare';
   participantId: string;
};

export type AutomaticScene = {
   type: 'automatic';
};

export type ViewableScene = GridScene | ScreenShareScene;
export type Scene = ViewableScene | AutomaticScene;

export type RoomSceneState = {
   isControlled: boolean;
   scene: Scene;
};

export type SynchronizedScenes = { [roomId: string]: RoomSceneState };

export type SceneViewModel = {
   scene: Scene;
   isApplied: boolean;
   isCurrent: boolean;
   id: string;
};
