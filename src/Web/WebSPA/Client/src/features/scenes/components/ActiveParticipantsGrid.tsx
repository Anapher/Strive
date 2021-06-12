import { makeStyles } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import useSomeParticipants from '../useSomeParticipants';
import ParticipantTile from './ParticipantTile';

export const ACTIVE_PARTICIPANTS_WEBCAM_RATIO = 9 / 16;
export const ACTIVE_PARTICIPANTS_MARGIN = 8;
export const ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE = 8;

const useStyles = makeStyles({
   root: {
      paddingRight: ACTIVE_PARTICIPANTS_MARGIN,
      height: '100%',
   },
});

type Props = {
   width: number;
   participant?: Participant;
};

export default function ActiveParticipantsGrid({ width, participant }: Props) {
   const classes = useStyles();
   const participants = useSomeParticipants(1, {
      includedParticipants: participant ? [participant] : undefined,
      activeOnly: true,
      webcamOnly: true,
   });

   const [mainTileSize, setMainTileSize] = useState<Size>({ width: 0, height: 0 });

   useEffect(() => {
      const mainTileWidth = width - ACTIVE_PARTICIPANTS_MARGIN;
      setMainTileSize({
         width: mainTileWidth,
         height: mainTileWidth * ACTIVE_PARTICIPANTS_WEBCAM_RATIO,
      });
   }, [width]);

   console.log(participants, 'participants');

   return (
      <div className={classes.root}>
         {participants.length > 0 && (
            <div style={{ ...mainTileSize, marginBottom: 8 }} key={participants[0].id}>
               <ParticipantTile {...mainTileSize} participant={participants[0]} />
            </div>
         )}
      </div>
   );
}
