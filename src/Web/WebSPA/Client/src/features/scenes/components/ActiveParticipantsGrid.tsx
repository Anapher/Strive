import { makeStyles } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import useSomeParticipants from '../useSomeParticipants';
import ParticipantTile from './ParticipantTile';

export const ACTIVE_PARTICIPANTS_WEBCAM_RATIO = 9 / 16;
export const ACTIVE_PARTICIPANTS_MARGIN = 0;
export const ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE = 8;

const useStyles = makeStyles({
   secondaryTiles: {
      display: 'flex',
      justifyContent: 'space-between',
   },
   root: {
      padding: ACTIVE_PARTICIPANTS_MARGIN,
      height: '100%',
   },
});

type Props = {
   width: number;
   fixedParticipants?: Participant[];
};

export default function ActiveParticipantsGrid({ width, fixedParticipants }: Props) {
   const classes = useStyles();
   const participants = useSomeParticipants(3, fixedParticipants);

   const [mainTileSize, setMainTileSize] = useState<Size>({ width: 0, height: 0 });
   const [secondaryTileSize, setSecondaryTileSize] = useState<Size>({ width: 0, height: 0 });

   useEffect(() => {
      const mainTileWidth = width - 2 * ACTIVE_PARTICIPANTS_MARGIN;
      setMainTileSize({
         width: mainTileWidth,
         height: mainTileWidth * ACTIVE_PARTICIPANTS_WEBCAM_RATIO,
      });

      const secondaryTileWidth = width - 2 * ACTIVE_PARTICIPANTS_MARGIN - ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE;
      setSecondaryTileSize({
         width: secondaryTileWidth,
         height: secondaryTileWidth * ACTIVE_PARTICIPANTS_WEBCAM_RATIO,
      });
   }, [width]);

   return (
      <div className={classes.root}>
         {participants.length > 0 && (
            <div style={{ ...mainTileSize }}>
               <ParticipantTile disableLayoutAnimation {...mainTileSize} participant={participants[0]} />
            </div>
         )}
         {participants.length > 1 && (
            <div className={classes.secondaryTiles}>
               <div style={{ ...secondaryTileSize }}>
                  <ParticipantTile disableLayoutAnimation {...secondaryTileSize} participant={participants[1]} />
               </div>
               {participants.length > 2 && (
                  <div style={{ ...secondaryTileSize }}>
                     <ParticipantTile disableLayoutAnimation {...secondaryTileSize} participant={participants[2]} />
                  </div>
               )}
            </div>
         )}
      </div>
   );
}
