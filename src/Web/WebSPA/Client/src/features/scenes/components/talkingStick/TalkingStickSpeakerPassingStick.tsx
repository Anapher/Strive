import { Divider, Fab, makeStyles } from '@material-ui/core';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import * as coreHub from 'src/core-hub';
import usePermission from 'src/hooks/usePermission';
import { SCENES_CAN_PASS_TALKING_STICK, SCENES_CAN_TAKE_TALKING_STICK } from 'src/permissions';
import { RenderSceneProps, TalkingStickScene } from '../../types';
import AddToListFab from './AddToListFab';
import PassStickList from './PassStickList';
import FrameWithPassFab from './FrameWithPassFab';
import TalkingStickScreen from './TalkingStickScreen';

const useStyles = makeStyles((theme) => ({
   participantList: {
      marginTop: theme.spacing(3),
      maxWidth: 260,
      marginLeft: 'auto',
      marginRight: 'auto',
   },
   primaryAction: {
      minWidth: 220,
   },
}));

export default function TalkingStickSpeakerPassingStick({
   className,
   next,
   scene,
}: RenderSceneProps<TalkingStickScene>) {
   const overwritten = next();
   if (overwritten) return <FrameWithPassFab>{overwritten}</FrameWithPassFab>;

   return <NoPresenter className={className} scene={scene} />;
}

type NoPresenterProps = {
   className?: string;
   scene: TalkingStickScene;
};

function NoPresenter({ className, scene }: NoPresenterProps) {
   const { t } = useTranslation();
   const dispatch = useDispatch();
   const classes = useStyles();

   const canTake = usePermission(SCENES_CAN_TAKE_TALKING_STICK);
   const canPass = usePermission(SCENES_CAN_PASS_TALKING_STICK);

   const handleTake = () => dispatch(coreHub.talkingStickTake());
   const handlePassStick = (participantId: string) => dispatch(coreHub.talkingStickPass(participantId));

   return (
      <TalkingStickScreen
         className={className}
         mode={scene.mode}
         footerChildren={
            canPass && (
               <div className={classes.participantList}>
                  <Divider variant="fullWidth" />
                  <PassStickList onPassStick={handlePassStick} />
               </div>
            )
         }
      >
         {canTake && (
            <Fab variant="extended" color="primary" className={classes.primaryAction} onClick={handleTake}>
               {t<string>('conference.scenes.talking_stick_modes.take_stick')}
            </Fab>
         )}
         {!canTake && <AddToListFab />}
      </TalkingStickScreen>
   );
}
