import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import React, { useContext } from 'react';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import { expandToBox } from '../../calculations';
import LayoutChildSizeContext from '../../layout-child-size-context';
import { ActiveSpeakerScene, RenderSceneProps } from '../../types';
import useSomeParticipants from '../../useSomeParticipants';
import ParticipantTile from '../ParticipantTile';
import TilesBarLayout from '../TilesBarLayout';

const MAIN_SPEAKER_MARGIN_TOP = 0;
const MAIN_SPEAKER_MARGIN_BOTTOM = 16;
const MAIN_SPEAKER_MARGIN_LEFT = 8;
const MAIN_SPEAKER_MARGIN_RIGHT = 8;

const useStyles = makeStyles((theme) => ({
   root: {
      display: 'flex',
      flexDirection: 'column',
   },
   chips: {
      marginTop: theme.spacing(1),
      marginBottom: theme.spacing(1),
      paddingRight: theme.spacing(2),
      height: 24,
   },
   content: {
      flex: 1,
      minHeight: 0,
   },
   mainParticipantTile: {
      width: '100%',
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'flex-start',
      paddingLeft: MAIN_SPEAKER_MARGIN_LEFT,
      paddingRight: MAIN_SPEAKER_MARGIN_RIGHT,
      paddingTop: MAIN_SPEAKER_MARGIN_TOP,
      paddingBottom: MAIN_SPEAKER_MARGIN_BOTTOM,
   },
}));

export default function RenderActiveSpeaker({ className, dimensions }: RenderSceneProps<ActiveSpeakerScene>) {
   const classes = useStyles();
   const activeParticipants = useSomeParticipants(16);
   if (activeParticipants.length === 0) return null;

   return (
      <div className={clsx(className, classes.root)}>
         <div className={classes.content}>
            <TilesBarLayout participants={activeParticipants.slice(1)} sceneSize={dimensions}>
               <RenderMainSpeakerTile participant={activeParticipants[0]} />
            </TilesBarLayout>
         </div>
      </div>
   );
}

type RenderMainSpeakerTileProps = {
   participant: Participant;
};

function RenderMainSpeakerTile({ participant }: RenderMainSpeakerTileProps) {
   const size = useContext(LayoutChildSizeContext);
   const classes = useStyles();

   const contentSize: Size = {
      width: size.width - MAIN_SPEAKER_MARGIN_LEFT - MAIN_SPEAKER_MARGIN_RIGHT,
      height: size.height - MAIN_SPEAKER_MARGIN_TOP - MAIN_SPEAKER_MARGIN_BOTTOM,
   };
   const fixedContentSize = expandToBox({ width: 16, height: 9 }, contentSize);

   return (
      <div className={classes.mainParticipantTile}>
         <div style={{ ...fixedContentSize }}>
            <ParticipantTile key={participant.id} {...fixedContentSize} participant={participant} />
         </div>
      </div>
   );
}
