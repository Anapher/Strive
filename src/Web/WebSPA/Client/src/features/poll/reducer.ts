import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DomainError, SuccessOrError } from 'src/communication-types';
import * as coreHub from 'src/core-hub';
import { SubmitPollAnswerDto } from 'src/core-hub.types';
import { parseSynchronizedObjectId } from 'src/store/signal/synchronization/synchronized-object-id';
import { POLL, POLL_ANSWERS, POLL_RESULT } from 'src/store/signal/synchronization/synchronized-object-ids';
import { synchronizeObjectState } from 'src/store/signal/synchronized-object';
import { SynchronizedPoll, SynchronizedPollAnswers, SynchronizedPollResults } from './types';

export type PollState = {
   polls: { [id: string]: SynchronizedPoll };
   results: { [id: string]: SynchronizedPollResults };
   answers: SynchronizedPollAnswers;
   creationDialogOpen: boolean;
   answerSubmissionErrors: Record<string, DomainError>;
};

const initialState: PollState = {
   polls: {},
   results: {},
   answers: { answers: {} },
   creationDialogOpen: false,
   answerSubmissionErrors: {},
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
      ...synchronizeObjectState(
         [
            { type: 'multiple', baseId: POLL, propertyName: 'polls' },
            { type: 'multiple', baseId: POLL_RESULT, propertyName: 'results' },
            { type: 'single', baseId: POLL_ANSWERS, propertyName: 'answers' },
         ],
         (state: PollState, syncObjId) => {
            const id = parseSynchronizedObjectId(syncObjId);
            if (id.id !== POLL) return;

            const pollId = id.parameters['pollId'];
            delete state.answerSubmissionErrors[pollId];
         },
      ),
      [coreHub.createPoll.returnAction]: (state, { payload }: PayloadAction<SuccessOrError>) => {
         if (payload.success) {
            state.creationDialogOpen = false;
         }
      },
      [coreHub.submitPollAnswer.returnAction]: (
         state,
         { payload, meta }: PayloadAction<SuccessOrError, string, { request: SubmitPollAnswerDto }>,
      ) => {
         if (!payload.success) {
            state.answerSubmissionErrors[meta.request.pollId] = payload.error;
         } else {
            delete state.answerSubmissionErrors[meta.request.pollId];
         }
      },
   },
});

export const { setCreationDialogOpen } = pollsSlice.actions;

export default pollsSlice.reducer;
