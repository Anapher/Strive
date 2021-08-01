import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SCENE, SCENE_TALKINGSTICK } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { ActiveParticipants, SynchronizedScene, SynchronizedTalkingStick } from './types';

export type SceneState = {
   synchronized: SynchronizedScene | null;
   talkingStick: SynchronizedTalkingStick | null;

   activeParticipants: ActiveParticipants;
};

const initialState: SceneState = {
   synchronized: null,
   talkingStick: null,
   activeParticipants: {},
};

const scenesSlice = createSlice({
   name: 'scenes',
   initialState,
   reducers: {
      setActiveParticipants(state, { payload }: PayloadAction<ActiveParticipants>) {
         state.activeParticipants = payload;
      },
   },
   extraReducers: {
      ...synchronizeObjectState([
         { type: 'single', baseId: SCENE, propertyName: 'synchronized' },
         { type: 'single', baseId: SCENE_TALKINGSTICK, propertyName: 'talkingStick' },
      ]),
   },
});

export const { setActiveParticipants } = scenesSlice.actions;
export default scenesSlice.reducer;
