import { Dialog, DialogTitle } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import presenters from '../scene-presenter-registry';
import { Scene } from '../types';

type Props = {
   onClose: () => void;
   open: boolean;

   availableScenes: Scene[];
   selectedScene: Scene;
   onChangeScene: (newScene: Scene) => void;
};

export default function SceneManagementModeSelectionDialog({
   onClose,
   open,
   availableScenes,
   selectedScene,
   onChangeScene,
}: Props) {
   const { t } = useTranslation();

   return (
      <Dialog onClose={onClose} open={open} aria-labelledby="scene-mode-dialog-title" maxWidth="sm" fullWidth>
         <DialogTitle id="scene-mode-dialog-title">{t('conference.scenes.mode_dialog.title')}</DialogTitle>
         {presenters.map(
            ({ ModeSceneListItem, type }) =>
               ModeSceneListItem && (
                  <ModeSceneListItem
                     key={type}
                     selectedScene={selectedScene}
                     availableScene={availableScenes.find((x) => x.type === type)}
                     onChangeScene={onChangeScene}
                  />
               ),
         )}
      </Dialog>
   );
}
