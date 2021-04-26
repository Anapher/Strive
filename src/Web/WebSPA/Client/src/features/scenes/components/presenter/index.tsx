import { ScenePresenter } from '../../types';
import PresenterScene from './PresenterScene';

const presenter: ScenePresenter = {
   type: 'presenter',
   RenderScene: PresenterScene,
};

export default presenter;
