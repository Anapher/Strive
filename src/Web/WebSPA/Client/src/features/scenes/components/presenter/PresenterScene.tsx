import { RenderSceneProps } from '../../types';

export default function PresenterScene({ next }: RenderSceneProps) {
   return <>{next()}</>;
}
