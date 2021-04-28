import { Fab, Grid, makeStyles, Portal } from '@material-ui/core';
import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import TwoLineFab from 'src/components/TwoLineFab';
import * as coreHub from 'src/core-hub';
import MediaControlsContext from 'src/features/media/media-controls-context';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_QUEUE_FOR_TALKING_STICK, SCENES_CAN_TAKE_TALKING_STICK } from 'src/permissions';
import { selectIsMePresenter, selectTalkingStickQueue } from '../../selectors';

const useStyles = makeStyles({
   primaryAction: {
      minWidth: 220,
   },
});

type Props = {
   children: React.ReactNode;
};

export default function TalkingStickFrame({ children }: Props) {
   const { t } = useTranslation();
   const classes = useStyles();
   const dispatch = useDispatch();

   const canQueue = usePermission(SCENES_CAN_QUEUE_FOR_TALKING_STICK);
   const canTake = usePermission(SCENES_CAN_TAKE_TALKING_STICK);

   const isPresenter = useSelector(selectIsMePresenter);
   const myId = useMyParticipantId();
   const queue = useSelector(selectTalkingStickQueue);
   const isInQueue = queue.includes(myId);

   const mediaContext = useContext(MediaControlsContext);

   const handleReturnStick = () => dispatch(coreHub.talkingStickReturn());
   const handleEnqueue = () => dispatch(coreHub.talkingStickEnqueue());
   const handleDequeue = () => dispatch(coreHub.talkingStickDequeue());
   const handleTake = () => dispatch(coreHub.talkingStickTake());

   const queuePosition = queue.indexOf(myId) + 1;
   const queueYoureNext = queuePosition === 1;

   return (
      <div>
         <Portal container={mediaContext.leftControlsContainer}>
            <Grid container spacing={1}>
               {isPresenter && (
                  <Grid item>
                     <Fab
                        variant="extended"
                        color="secondary"
                        className={classes.primaryAction}
                        onClick={handleReturnStick}
                     >
                        {t<string>('conference.scenes.talking_stick_modes.return_stick')}
                     </Fab>
                  </Grid>
               )}
               {canQueue && !isInQueue && (
                  <Grid item>
                     <TwoLineFab
                        variant="extended"
                        color="secondary"
                        className={classes.primaryAction}
                        onClick={handleEnqueue}
                        subtitle={t('conference.scenes.talking_stick_modes.enqueue_status', { count: queue.length })}
                     >
                        {t<string>('conference.scenes.talking_stick_modes.enqueue')}
                     </TwoLineFab>
                  </Grid>
               )}
               {isInQueue && (
                  <Grid item>
                     <TwoLineFab
                        variant="extended"
                        color="secondary"
                        className={classes.primaryAction}
                        onClick={handleDequeue}
                        subtitle={
                           queueYoureNext
                              ? t('conference.scenes.talking_stick_modes.queue_youre_next')
                              : t('conference.scenes.talking_stick_modes.dequeue_status', {
                                   total: queue.length,
                                   pos: queuePosition,
                                })
                        }
                     >
                        {t<string>('conference.scenes.talking_stick_modes.dequeue')}
                     </TwoLineFab>
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
