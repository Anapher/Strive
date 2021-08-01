import React from 'react';
import scenePresenters from '../scene-presenter-registry';
import { RenderSceneProps, Scene } from '../types';

type SceneSelectorProps = Omit<Omit<RenderSceneProps, 'next'>, 'scene'> & { sceneStack: Scene[] };

export default function SceneSelector({ sceneStack, ...props }: SceneSelectorProps) {
   const scene = sceneStack[0];

   const presenter = scenePresenters.find((x) => x.type === scene.type);
   if (!presenter) return null;

   const next = (additional?: any) => {
      const otherScenes = sceneStack.slice(1);

      if (otherScenes.length === 0) return null;
      return <SceneSelector {...props} {...additional} sceneStack={otherScenes} />;
   };

   return <presenter.RenderScene {...props} scene={scene} next={next} />;
}
