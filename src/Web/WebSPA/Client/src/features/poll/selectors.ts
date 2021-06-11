import { createSelector } from '@reduxjs/toolkit';
import { DomainError } from 'src/communication-types';
import { RootState } from 'src/store';
import { PollViewModel } from './types';

const selectMyPolls = (state: RootState) => state.poll.polls;
const selectMyAnswers = (state: RootState) => state.poll.answers.answers;
const selectMyPollResults = (state: RootState) => state.poll.results;

export const selectAnswerSubmissionError = (state: RootState, pollId: string) =>
   state.poll.answerSubmissionErrors[pollId] as DomainError | undefined;

export const selectPollViewModels = createSelector(
   selectMyPolls,
   selectMyAnswers,
   selectMyPollResults,
   (polls, answers, results) => {
      const resultsList = Object.values(results);

      return Object.values(polls).map<PollViewModel>((poll) => ({
         poll,
         answer: answers[poll.id],
         results: resultsList.find((x) => x.pollId === poll.id),
      }));
   },
);

// this selector is safe as no new obj is created
export const selectPoll = (state: RootState, pollId: string) =>
   Object.values(selectMyPolls(state)).find((x) => x.id === pollId);

export const selectPollResults = (state: RootState, pollId: string) =>
   Object.values(selectMyPollResults(state)).find((x) => x.pollId === pollId);

export const selectPollAnswer = (state: RootState, pollId: string) => selectMyAnswers(state)[pollId];

export const selectPollViewModelFactory = () =>
   createSelector(selectPoll, selectPollAnswer, selectPollResults, (poll, answer, results) => {
      if (!poll) return undefined;

      return { poll, results, answer } as PollViewModel;
   });
