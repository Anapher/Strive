import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SCENE } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { ActiveParticipants, FollowServer, Scene, SceneConfig, SynchronizedScene, ViewableScene } from './types';

export type SceneState = {
   synchronized: SynchronizedScene | null;

   appliedScene: Scene | FollowServer;
   currentScene: ViewableScene;
   sceneConfig: SceneConfig;

   /** participants that should currently be shown, because they are speaking or because they are presentator. The value is the last activity time. */
   activeParticipants: ActiveParticipants;
};

const initialState: SceneState = {
   synchronized: null,

   /** the scene that is selected. can be changed by the user */
   appliedScene: { type: 'followServer' },

   /** the scene that is displayed. can not be changed by the user */
   currentScene: { type: 'grid' },

   sceneConfig: { hideParticipantsWithoutWebcam: false },

   activeParticipants: {},
};

const isViewableScene = (scene: Scene | FollowServer): scene is ViewableScene =>
   scene.type !== 'autonomous' && scene.type !== 'followServer';

const scenesSlice = createSlice({
   name: 'scenes',
   initialState,
   reducers: {
      setAppliedScene(state, { payload }: PayloadAction<Scene>) {
         state.appliedScene = payload;

         if (isViewableScene(payload)) {
            state.currentScene = payload;
         }
      },
      setCurrentScene(state, { payload }: PayloadAction<ViewableScene>) {
         state.currentScene = payload;
      },
      addActiveParticipant(
         state,
         { payload: { participantId, orderNumber } }: PayloadAction<{ participantId: string; orderNumber: number }>,
      ) {
         state.activeParticipants[participantId] = { orderNumber };
      },
      updateActiveParticipantDeleted(
         state,
         {
            payload: { participantId, deletedOn },
         }: PayloadAction<{ participantId: string; deletedOn: string | undefined }>,
      ) {
         state.activeParticipants[participantId] = { ...state.activeParticipants[participantId], deletedOn };
      },
      removeActiveParticipant(state, { payload }: PayloadAction<string>) {
         delete state.activeParticipants[payload];
      },
   },
   extraReducers: {
      ...synchronizeObjectState({ type: 'single', baseId: SCENE, propertyName: 'synchronized' }),
   },
});

export const {
   setAppliedScene,
   setCurrentScene,
   addActiveParticipant,
   updateActiveParticipantDeleted,
   removeActiveParticipant,
} = scenesSlice.actions;
export default scenesSlice.reducer;
