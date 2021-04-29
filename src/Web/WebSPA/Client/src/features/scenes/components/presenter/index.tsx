import { useTranslation } from 'react-i18next';
import { useSelector } from 'react-redux';
import { selectParticipant } from 'src/features/conference/selectors';
import { RootState } from 'src/store';
import { ActiveDescriptorProps, PresenterScene, ScenePresenter } from '../../types';
import RenderPresenter from './RenderPresenter';

function ActiveDescriptor({ scene }: ActiveDescriptorProps<PresenterScene>) {
   const { t } = useTranslation();
   const participant = useSelector((state: RootState) => selectParticipant(state, scene.presenterParticipantId));

   return <span>{t('conference.scenes.presenter_text', { name: participant?.displayName })}</span>;
}

const presenter: ScenePresenter<PresenterScene> = {
   type: 'presenter',
   RenderScene: RenderPresenter,
   getAutoHideMediaControls: () => false,
   ActiveDescriptor,
};

export default presenter;
