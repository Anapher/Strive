import { makeStyles } from '@material-ui/core';
import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { selectActiveParticipants } from '../../selectors';
import { RenderSceneProps } from '../../types';
import ParticipantTile from '../ParticipantTile';
import clsx from 'classnames';
import { expandToBox } from '../../calculations';
import { Size } from 'src/types';

const useStyles = makeStyles({
   root: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
});

const getListeningParticipantsWidth = (width: number) => {
   if (width <= 400) return 80;
   if (width <= 800) return 160;

   return 200;
};

export default function ActiveSpeakerScene({ className, dimensions, setAutoHideControls }: RenderSceneProps) {
   const classes = useStyles();

   const activeParticipants = useSelector(selectActiveParticipants);
   const participants = useSelector(selectParticipants);

   useEffect(() => {
      setAutoHideControls(true);
   }, []);

   if (participants.length === 0) return null;

   const tileWidth = getListeningParticipantsWidth(dimensions.width);
   const tileHeight = (tileWidth / 16) * 9;

   const activeParticipantDimensions: Size = {
      width: dimensions.width - 16,
      height: dimensions.height - 8 - 8 - 16 - tileHeight,
   };

   const size = expandToBox({ height: 9, width: 16 }, activeParticipantDimensions);
   const smallTileCount = (dimensions.width - 8) / (tileWidth + 8);

   return (
      <div className={clsx(className, classes.root)}>
         <div style={{ margin: 8, ...size }}>
            <ParticipantTile size={size} participant={participants[0]} />
         </div>
         <div style={{ display: 'flex', marginTop: 8 }}>
            {Array.from({ length: smallTileCount }).map((_, i) => (
               <div key={i} style={{ backgroundColor: 'red', width: tileWidth, height: tileHeight, marginLeft: 8 }} />
            ))}
         </div>
      </div>
   );
}
