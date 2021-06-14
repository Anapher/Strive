import React from 'react';
import Whiteboard from 'src/features/whiteboard/components/Whiteboard';
import { RenderSceneProps, WhiteboardScene } from '../../types';

export default function RenderWhiteboard({ className, scene }: RenderSceneProps<WhiteboardScene>) {
   return <Whiteboard />;
}
