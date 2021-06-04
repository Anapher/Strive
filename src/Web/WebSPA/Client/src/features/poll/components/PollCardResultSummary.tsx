import React from 'react';
import { PollViewModel } from '../types';

type Props = {
   poll: PollViewModel;
};

export default function PollCardResultSummary({ poll }: Props) {
   return <div>{poll.poll.id}</div>;
}
