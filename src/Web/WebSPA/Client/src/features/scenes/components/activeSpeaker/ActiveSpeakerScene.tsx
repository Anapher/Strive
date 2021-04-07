import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Size } from 'src/types';
import { expandToBox } from '../../calculations';
import { RenderSceneProps } from '../../types';
import useSomeParticipants from '../../useSomeParticipants';
import ParticipantTile from '../ParticipantTile';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

const getListeningParticipantsWidth = (width: number) => {
   if (width <= 400) return 100;
   if (width <= 800) return 180;
   if (width <= 1200) return 260;

   return 340;
};

export default function ActiveSpeakerScene({ className, dimensions, setAutoHideControls }: RenderSceneProps) {
   const classes = useStyles();

   useEffect(() => {
      setAutoHideControls(true);
   }, []);

   const tileWidth = getListeningParticipantsWidth(dimensions.width);
   const tileHeight = (tileWidth / 16) * 9;

   const activeParticipantDimensions: Size = {
      width: dimensions.width - 16,
      height: dimensions.height - 8 - 8 - 16 - tileHeight,
   };

   const size = expandToBox({ height: 9, width: 16 }, activeParticipantDimensions);
   const smallTileCount = (dimensions.width - 8) / (tileWidth + 8);

   const activeParticipants = useSomeParticipants(smallTileCount);
   const participants = useSelector(selectParticipants);

   if (participants.length === 0) return null;

   return (
      <div className={clsx(className, classes.root)}>
         <div style={{ margin: 8, ...size }}>
            <ParticipantTile {...size} participant={activeParticipants[0]} disableLayoutAnimation />
         </div>
         <div style={{ display: 'flex', marginTop: 8 }}>
            {activeParticipants.slice(1).map((participant, i) => (
               <div style={{ width: tileWidth, height: tileHeight, marginLeft: i === 0 ? 8 : 0 }} key={participant.id}>
                  <ParticipantTile
                     width={tileWidth}
                     height={tileHeight}
                     participant={participant}
                     disableLayoutAnimation
                  />
               </div>
            ))}
         </div>
      </div>
   );
}
