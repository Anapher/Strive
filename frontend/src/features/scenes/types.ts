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

export type ActiveSceneMenuItemProps = {
   onClose: () => void;
};

export type ActiveSceneInfo = {
   useIsActive: () => boolean;
   ActiveMenuItem: React.ComponentType;
   OpenMenuItem: React.ComponentType<ActiveSceneMenuItemProps>;
   AlwaysRender?: React.ComponentType;
};

export type ActiveParticipants = { [participantId: string]: { deletedOn?: string } };
