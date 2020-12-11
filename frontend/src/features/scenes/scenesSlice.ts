import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { createSynchronizeObjectReducer } from 'src/store/signal/synchronized-object';
import { ActiveParticipants, Scene, SynchronizedScenes, ViewableScene } from './types';

export type RoomsState = {
   synchronized: SynchronizedScenes | null;

   appliedScene: Scene;
   currentScene: ViewableScene;

   /** participants that should currently be shown, because they are speaking or because they are presentator. The value is the last activity time. */
   activeParticipants: ActiveParticipants;
};

const initialState: RoomsState = {
   synchronized: null,

   /** the scene that is selected. can be changed by the user */
   appliedScene: { type: 'automatic' },

   /** the scene that is displayed. can not be changed by the user */
   currentScene: { type: 'grid' },

   activeParticipants: {},
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
      addActiveParticipant(state, { payload }: PayloadAction<string>) {
         state.activeParticipants[payload] = {};
      },
      updateActiveParticipantDeleted(
         state,
         {
            payload: { participantId, deletedOn },
         }: PayloadAction<{ participantId: string; deletedOn: string | undefined }>,
      ) {
         state.activeParticipants[participantId] = { deletedOn };
      },
      removeActiveParticipant(state, { payload }: PayloadAction<string>) {
         delete state.activeParticipants[payload];
      },
   },
   extraReducers: {
      ...createSynchronizeObjectReducer({ name: 'scenes', stateName: 'synchronized' }),
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
