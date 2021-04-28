import { RenderSceneProps } from '../../types';

export default function RenderAutonomous({ next }: RenderSceneProps) {
   return <>{next()}</>;
}
