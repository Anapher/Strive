import React from 'react';
import { ListItemPopperProps } from '../SceneListItemWithPopper';

export default function PollOptionsPopper({ anchorEl, scene, ...props }: ListItemPopperProps) {
   if (scene.type !== 'whiteboard') throw new Error('Invalid scene');

   return <div />;
}
