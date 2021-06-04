import React from 'react';
import { useSelector } from 'react-redux';
import PollContextMenu from 'src/features/poll/components/PollContextMenu';
import { selectPollViewModel } from 'src/features/poll/selectors';
import { RootState } from 'src/store';
import { ListItemPopperProps } from '../SceneListItemWithPopper';

export default function PollOptionsPopper({ anchorEl, scene, ...props }: ListItemPopperProps) {
   if (scene.type !== 'poll') throw new Error('Invalid scene');
   const poll = useSelector((state: RootState) => selectPollViewModel(state, scene.pollId));

   if (!poll) return null;
   return <PollContextMenu {...props} anchorEl={anchorEl as any} poll={poll} />;
}
