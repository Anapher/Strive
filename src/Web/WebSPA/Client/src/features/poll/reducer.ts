import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { POLL, POLL_ANSWERS, POLL_RESULT } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { SynchronizedPoll, SynchronizedPollAnswers, SynchronizedPollResults } from './types';

export type PollState = {
   polls: { [id: string]: SynchronizedPoll };
   results: { [id: string]: SynchronizedPollResults };
   answers: SynchronizedPollAnswers;
   creationDialogOpen: boolean;
};

const initialState: PollState = {
   polls: {},
   results: {},
   answers: { answers: {} },
   creationDialogOpen: false,
};

const pollsSlice = createSlice({
   name: 'polls',
   initialState,
   reducers: {
      setCreationDialogOpen(state, { payload }: PayloadAction<boolean>) {
         state.creationDialogOpen = payload;
      },
   },
   extraReducers: {
      ...synchronizeObjectState([
         { type: 'multiple', baseId: POLL, propertyName: 'polls' },
         { type: 'multiple', baseId: POLL_RESULT, propertyName: 'results' },
         { type: 'single', baseId: POLL_ANSWERS, propertyName: 'answers' },
      ]),
      [coreHub.createPoll.returnAction]: (state, { payload }: PayloadAction<SuccessOrError>) => {
         if (payload.success) {
            state.creationDialogOpen = false;
         }
      },
   },
});

export const { setCreationDialogOpen } = pollsSlice.actions;

export default pollsSlice.reducer;
