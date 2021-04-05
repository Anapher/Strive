import React from 'react';
import scenePresenters from '../scene-presenter-registry';
import { RenderSceneProps } from '../types';

export default function SceneSelector(props: RenderSceneProps) {
   const presenter = scenePresenters.find((x) => x.type === props.scene.type);
   if (!presenter) return null;

   return <presenter.RenderScene {...props} />;
}
