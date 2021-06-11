import React from 'react';
import PollContextMenu from 'src/features/poll/components/PollContextMenu';
import { selectPollViewModelFactory } from 'src/features/poll/selectors';
import useSelectorFactory from 'src/hooks/useSelectorFactory';
import { RootState } from 'src/store';
import { ListItemPopperProps } from '../SceneListItemWithPopper';

export default function PollOptionsPopper({ anchorEl, scene, ...props }: ListItemPopperProps) {
   if (scene.type !== 'poll') throw new Error('Invalid scene');
   const poll = useSelectorFactory(selectPollViewModelFactory, (state: RootState, selector) =>
      selector(state, scene.pollId),
   );

   if (!poll) return null;
   return <PollContextMenu {...props} anchorEl={anchorEl as any} poll={poll} />;
}
