import { Fab } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_QUEUE_FOR_TALKING_STICK } from 'src/permissions';
import { RenderSceneProps } from '../../types';
import TalkingStickFrame from './TalkingStickFrame';
import TalkingStickScreen from './TalkingStickScreen';

export default function TalkingStickQueue({ className, next }: RenderSceneProps) {
   const overwritten = next();
   if (overwritten) return <TalkingStickFrame>{overwritten}</TalkingStickFrame>;

   return <QueueNoPresenter className={className} />;
}

type QueueNoPresenterProps = {
   className?: string;
};

function QueueNoPresenter({ className }: QueueNoPresenterProps) {
   const { t } = useTranslation();
   const dispatch = useDispatch();
   const canEnqueue = usePermission(SCENES_CAN_QUEUE_FOR_TALKING_STICK);

   const handleEnqueue = () => dispatch(coreHub.talkingStickEnqueue());

   return (
      <TalkingStickScreen className={className} mode="queue">
         <Fab
            variant="extended"
            color="primary"
            style={{ minWidth: 220 }}
            disabled={!canEnqueue}
            onClick={handleEnqueue}
         >
            {t<string>('conference.scenes.talking_stick_modes.take_stick')}
         </Fab>
      </TalkingStickScreen>
   );
}
