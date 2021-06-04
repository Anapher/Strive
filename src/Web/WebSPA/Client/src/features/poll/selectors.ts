import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'src/store';
import { PollViewModel } from './types';

const getPollId = (_: unknown, id: string) => id;

const selectMyPolls = (state: RootState) => Object.values(state.poll.polls);
const selectMyAnswers = (state: RootState) => state.poll.answers.answers;
const selectMyPollResults = (state: RootState) => Object.values(state.poll.results);

export const selectPollViewModels = createSelector(
   selectMyPolls,
   selectMyAnswers,
   selectMyPollResults,
   (polls, answers, results) => {
      return polls.map<PollViewModel>((poll) => ({
         poll,
         answer: answers[poll.id],
         results: results.find((x) => x.pollId === poll.id),
      }));
   },
);

export const selectPoll = (state: RootState, pollId: string) =>
   Object.values(state.poll.polls).find((x) => x.id === pollId);

export const selectPollViewModel = createSelector(
   selectMyPolls,
   selectMyAnswers,
   selectMyPollResults,
   getPollId,
   (polls, answers, pollResults, pollId) => {
      const poll = polls.find((x) => x.id === pollId);
      if (!poll) return undefined;

      const results = pollResults.find((x) => x.pollId === poll.id);
      const answer = answers[poll.id];

      return { poll, results, answer } as PollViewModel;
   },
);
