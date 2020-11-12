export type ParticipantDto = {
   participantId: string;
   role: string;
   displayName?: string;
   attributes: { [key: string]: string };
};

export type Scene = GridScene | ScreenShareScene;

export type GridScene = {
   type: 'grid';
   hideParticipantsWithoutWebcam?: boolean;
};

export type ScreenShareScene = {
   type: 'screenshare';
   participantId: string;
};
