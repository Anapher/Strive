import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SCENE, SCENE_TALKINGSTICK } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { ActiveParticipants, SynchronizedScene, SynchronizedTalkingStick } from './types';

export type SceneState = {
   synchronized: SynchronizedScene | null;
   talkingStick: SynchronizedTalkingStick | null;

   /** participants that should currently be shown, because they are speaking or because they are presentator. The value is the last activity time. */
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
      ...synchronizeObjectState([
         { type: 'single', baseId: SCENE, propertyName: 'synchronized' },
         { type: 'single', baseId: SCENE_TALKINGSTICK, propertyName: 'talkingStick' },
      ]),
   },
});

export const { addActiveParticipant, updateActiveParticipantDeleted, removeActiveParticipant } = scenesSlice.actions;
export default scenesSlice.reducer;
