import { makeStyles, Typography } from '@material-ui/core';
import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import { selectActiveParticipantsWithWebcam } from '../selectors';
import ParticipantTile from './ParticipantTile';

export const ACTIVE_PARTICIPANTS_WEBCAM_RATIO = 9 / 16;
export const ACTIVE_PARTICIPANTS_MARGIN = 0;
export const ACTIVE_PARTICIPANTS_SECONDARY_TILES_SPACE = 8;

const useStyles = makeStyles({
   secondaryTiles: {
      display: 'flex',
      justifyContent: 'space-between',
   },
});

type Props = {
   width: number;
};

export default function ActiveParticipantsGrid({ width }: Props) {
   const classes = useStyles();

   const activeParticipants = useSelector(selectActiveParticipantsWithWebcam);
   const participants = useSelector(selectParticipants);

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

   if (!participants) {
      return <Typography>Yikes</Typography>;
   }

   const participantDtos = activeParticipants
      .map((participantId) => participants.find((x) => x.id === participantId))
      .filter((x): x is Participant => !!x);

   return (
      <div style={{ padding: ACTIVE_PARTICIPANTS_MARGIN, height: '100%' }}>
         {participantDtos.length > 0 && (
            <div style={{ ...mainTileSize }}>
               <ParticipantTile disableLayoutAnimation {...mainTileSize} participant={participantDtos[0]} />
            </div>
         )}
         {participantDtos.length > 1 && (
            <div className={classes.secondaryTiles}>
               <div style={{ ...secondaryTileSize }}>
                  <ParticipantTile disableLayoutAnimation {...secondaryTileSize} participant={participantDtos[1]} />
               </div>
               {participantDtos.length > 2 && (
                  <div style={{ ...secondaryTileSize }}>
                     <ParticipantTile disableLayoutAnimation {...secondaryTileSize} participant={participantDtos[2]} />
                  </div>
               )}
            </div>
         )}
      </div>
   );
}
