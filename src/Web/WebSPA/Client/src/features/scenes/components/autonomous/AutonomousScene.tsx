import React from 'react';
import { useSelector } from 'react-redux';
import { selectCurrentScene } from '../../selectors';
import { RenderSceneProps } from '../../types';
import SceneSelector from '../SceneSelector';

export default function AutonomousScene(props: RenderSceneProps) {
   const currentScene = useSelector(selectCurrentScene);
   return <SceneSelector {...props} scene={currentScene} />;
}
