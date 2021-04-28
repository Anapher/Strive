import { Fab, Grid, Portal } from '@material-ui/core';
import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import MediaControlsContext from 'src/features/media/media-controls-context';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_PASS_TALKING_STICK, SCENES_CAN_TAKE_TALKING_STICK } from 'src/permissions';
import { selectIsMePresenter } from '../../selectors';
import AddToListFab from './AddToListFab';
import PassStickFab from './PassStickFab';

type FrameProps = {
   children: React.ReactNode;
};

export default function FrameWithPassFab({ children }: FrameProps) {
   const { t } = useTranslation();
   const dispatch = useDispatch();

   const canPass = usePermission(SCENES_CAN_PASS_TALKING_STICK);
   const canTake = usePermission(SCENES_CAN_TAKE_TALKING_STICK);

   const isPresenter = useSelector(selectIsMePresenter);

   const mediaContext = useContext(MediaControlsContext);

   const handleReturnStick = () => dispatch(coreHub.talkingStickReturn());
   const handleTake = () => dispatch(coreHub.talkingStickTake());

   return (
      <div>
         <Portal container={mediaContext.leftControlsContainer}>
            <Grid container spacing={1}>
               {canPass && (
                  <Grid item>
                     <PassStickFab />
                  </Grid>
               )}
               {isPresenter && (
                  <Grid item>
                     <Fab variant="extended" color="secondary" onClick={handleReturnStick}>
                        {t<string>('conference.scenes.talking_stick_modes.return_stick')}
                     </Fab>
                  </Grid>
               )}
               {!isPresenter && !canPass && (
                  <Grid item>
                     <AddToListFab />
                  </Grid>
               )}
               {canTake && !isPresenter && (
                  <Grid item>
                     <Fab variant="extended" color="primary" onClick={handleTake}>
                        {t<string>('conference.scenes.talking_stick_modes.take_stick')}
                     </Fab>
                  </Grid>
               )}
            </Grid>
         </Portal>
         {children}
      </div>
   );
}
