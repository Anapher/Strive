import { PresenterScene, ScenePresenter } from '../../types';
import RenderPresenter from './RenderPresenter';

const presenter: ScenePresenter<PresenterScene> = {
   type: 'presenter',
   RenderScene: RenderPresenter,
   getAutoHideMediaControls: () => false,
};

export default presenter;
