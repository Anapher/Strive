import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { Scene, SynchronizedScenes, ViewableScene } from './types';

export type RoomsState = {
   synchronized: SynchronizedScenes | null;

   appliedScene: Scene;
   currentScene: ViewableScene;
};

const initialState: RoomsState = {
   synchronized: null,

   /** the scene that is selected. can be changed by the user */
   appliedScene: { type: 'automatic' },

   /** the scene that is displayed. can not be changed by the user */
   currentScene: { type: 'grid' },
};

const scenesSlice = createSlice({
   name: 'rooms',
   initialState,
   reducers: {
      setAppliedScene(state, { payload }: PayloadAction<Scene>) {
         state.appliedScene = payload;

         if (payload.type !== 'automatic') {
            state.currentScene = payload;
         }
      },
      setCurrentScene(state, { payload }: PayloadAction<ViewableScene>) {
         state.currentScene = payload;
      },
   },
   extraReducers: {
      ...createSynchronizeObjectReducer({ name: 'scenes', stateName: 'synchronized' }),
   },
});

export const { setAppliedScene, setCurrentScene } = scenesSlice.actions;
export default scenesSlice.reducer;
