import React from 'react';
import { PollViewModel } from '../types';
import pollTypes from '../types/register';

type Props = {
   viewModel: PollViewModel;
};

export default function PollResultsView({ viewModel }: Props) {
   const presenter = pollTypes.find((x) => x.instructionType === viewModel.poll.instruction.type);
   if (!presenter) return null;
   if (!viewModel.results) return null;

   return <presenter.ResultsView viewModel={viewModel} />;
}
