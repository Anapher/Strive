import { createSelector } from '@reduxjs/toolkit';
import _ from 'lodash';
import { RootState } from 'src/store';
import { selectParticipants } from '../conference/selectors';
import { selectStreams } from '../media/selectors';
import { Scene, SceneViewModel } from './types';

export const selectAppliedScene = (state: RootState) => {
   const appScene = state.scenes.appliedScene;
   if (appScene.type === 'followServer') {
      const serverScene = state.scenes.synchronized?.active.scene;
      return serverScene ?? { type: 'autonomous' };
   }

   return appScene;
};

export const selectCurrentScene = (state: RootState) => state.scenes.currentScene;

export const selectActiveParticipants = (state: RootState) => state.scenes.activeParticipants;
export const selectAvailableScenes = (state: RootState) => state.scenes.synchronized?.availableScenes ?? [];

export const selectServerScene = (state: RootState) => state.scenes.synchronized?.active;

export const selectActiveParticipantsWithWebcam = createSelector(
   selectActiveParticipants,
   selectStreams,
   (participants, streams) => {
      if (!streams) return [];

      return _(Object.entries(participants))
         .filter(([participantId]) => streams[participantId]?.producers.webcam?.paused === false)
         .orderBy(([, info]) => info.orderNumber, 'asc')
         .map(([participantId]) => participantId)
         .value();
   },
);

export const selectAvailableScenesViewModels = createSelector(
   selectAvailableScenes,
   selectAppliedScene,
   selectCurrentScene,
   (scenes, applied, current) => {
      const appliedSceneId = getSceneId(applied);
      const currentSceneId = getSceneId(current);

      return scenes.map<SceneViewModel>((scene) => {
         const id = getSceneId(scene);

         return { scene, id, isApplied: id === appliedSceneId, isCurrent: id === currentSceneId };
      });
   },
);

export const selectSceneOverlayParticipants = createSelector(
   selectCurrentScene,
   selectParticipants,
   (scene, participants) => {
      if (scene.type === 'screenShare') {
         return participants?.filter((x) => x.id === scene.participantId) ?? [];
      }

      return [];
   },
);

export const getSceneId = (scene: Scene) => {
   switch (scene.type) {
      case 'screenShare':
         return `${scene.type}::${scene.participantId}`;
      default:
         return scene.type;
   }
};
