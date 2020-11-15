import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { selectScreenSharingParticipants } from '../media/selectors';
import { selectParticipantRoom } from '../rooms/selectors';
import { Scene, SceneViewModel } from './types';

export const ALWAYS_AVAILABLE_SCENES: Scene[] = [{ type: 'automatic' }, { type: 'grid' }];

export const selectAppliedScene = (state: RootState) => state.scenes.appliedScene;
export const selectScenes = (state: RootState) => state.scenes.synchronized;
export const selectCurrentScene = (state: RootState) => state.scenes.currentScene;

export const selectServerProvidedScene = createSelector(selectParticipantRoom, selectScenes, (room, scenes) => {
   if (!room) return undefined;
   if (!scenes) return undefined;

   return scenes[room];
});

export const selectAvailableScenes = createSelector(selectScreenSharingParticipants, (participants) => {
   return [
      ...ALWAYS_AVAILABLE_SCENES,
      ...(participants?.map<Scene>((x) => ({ type: 'screenshare', participantId: x.participantId })) || []),
   ];
});

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

export const getSceneId = (scene: Scene) => {
   switch (scene.type) {
      case 'grid':
         return scene.type;
      case 'automatic':
         return scene.type;
      case 'screenshare':
         return `${scene.type}::${scene.participantId}`;
   }
};
