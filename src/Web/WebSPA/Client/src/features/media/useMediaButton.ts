import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import { showMessage } from 'src/store/notifier/actions';
import { UseMediaState } from 'src/store/webrtc/hooks/useMedia';
import { formatErrorMessage } from 'src/utils/error-utils';

export default function useMediaButton(
   pauseOnToggle: boolean,
   { enable, disable, pause, resume, enabled, paused }: UseMediaState,
   translationKey: 'screen' | 'webcam' | 'mic',
) {
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const handleClick = async () => {
      try {
         if (!enabled) {
            await enable();
         } else {
            if (paused) {
               await resume();
            } else {
               if (pauseOnToggle) {
                  await pause();
               } else {
                  await disable();
               }
            }
         }
      } catch (error) {
         if (error.message) {
            dispatch(showMessage({ message: formatErrorMessage(error), type: 'error' }));
         } else {
            dispatch(showMessage({ message: error?.toString(), type: 'error' }));
         }
      }
   };

   let title: string;
   if (enabled) {
      if (paused) title = t(`conference.media.controls.${translationKey}.paused`);
      else title = t(`conference.media.controls.${translationKey}.active`);
   } else {
      title = t(`conference.media.controls.${translationKey}.disabled`);
   }

   const label =
      enabled && !paused
         ? t(`conference.media.controls.${translationKey}.label_disable`)
         : t(`conference.media.controls.${translationKey}.label_enable`);

   return { title, label, id: `media-controls-${translationKey}`, handleClick, activated: enabled && !paused };
}
