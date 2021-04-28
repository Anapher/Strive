import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import TwoLineFab from 'src/components/TwoLineFab';
import * as coreHub from 'src/core-hub';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_QUEUE_FOR_TALKING_STICK } from 'src/permissions';
import { selectTalkingStickQueue } from '../../selectors';

const useStyles = makeStyles((theme) => ({
   primaryAction: {
      minWidth: 220,
      padding: theme.spacing(0, 4),
   },
}));

export default function AddToListFab() {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const canEnqueue = usePermission(SCENES_CAN_QUEUE_FOR_TALKING_STICK);
   const myId = useMyParticipantId();
   const queue = useSelector(selectTalkingStickQueue);
   const isInQueue = queue.includes(myId);

   const handleEnqueue = () => dispatch(coreHub.talkingStickEnqueue());
   const handleDequeue = () => dispatch(coreHub.talkingStickDequeue());

   return (
      <TwoLineFab
         variant="extended"
         color={isInQueue ? 'secondary' : 'primary'}
         disabled={!isInQueue && !canEnqueue}
         className={classes.primaryAction}
         onClick={isInQueue ? handleDequeue : handleEnqueue}
         subtitle={t('conference.scenes.talking_stick_modes.add_to_list_status', {
            count: queue.length - (isInQueue ? 1 : 0),
         })}
      >
         {isInQueue
            ? t<string>('conference.scenes.talking_stick_modes.remove_from_list')
            : t<string>('conference.scenes.talking_stick_modes.add_to_list')}
      </TwoLineFab>
   );
}
