import { makeStyles } from '@material-ui/core';
import React from 'react';
import { Size } from 'src/types';
import { expandToBox, maxWidth } from '../calculations';
import PresentationSceneParticipants from './PresentationSceneParticipants';

const useStyles = makeStyles(() => ({
   container: {
      width: '100%',
      height: '100%',
      position: 'relative',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'flex-start',
      flexDirection: 'column',
   },
}));

type Props = {
   className?: string;
   dimensions: Size;

   contentRatio: Size;
   maxContentWidth?: number;

   showParticipants?: boolean;

   participantTileWidth?: number;
   participantTileHeight?: number;

   maxOverlayFactor?: number;

   render: (size: Size) => React.ReactNode;
};

export default function PresentationScene({
   className,
   contentRatio,
   maxContentWidth,
   dimensions,
   render,
   showParticipants = true,
   participantTileWidth = 16 * 18,
   participantTileHeight = 9 * 18,
   maxOverlayFactor = 0.33,
}: Props) {
   const classes = useStyles();

   // measure
   let computedSize = expandToBox(contentRatio, dimensions);
   if (maxContentWidth) computedSize = maxWidth(computedSize, maxContentWidth);

   let participantsPlace: 'bottom' | 'right' | undefined;

   if (showParticipants) {
      // compute the vertical/horizontal margin if we would use the full size content
      const marginBottom = dimensions.height - computedSize.height;
      const marginRight = dimensions.width - computedSize.width;

      const neededPlaceBottom = participantTileHeight * (1 - maxOverlayFactor);
      const neededPlaceRight = participantTileWidth * (1 - maxOverlayFactor);

      const missingPlaceBottom = Math.max(0, neededPlaceBottom - marginBottom);
      const missingPlaceRight = Math.max(0, neededPlaceRight - marginRight);

      let newDimensions: Size | undefined;
      if (missingPlaceRight > missingPlaceBottom) {
         participantsPlace = 'bottom';

         if (missingPlaceBottom > 0) {
            newDimensions = { width: computedSize.width, height: computedSize.height - missingPlaceBottom };
         }
      } else {
         participantsPlace = 'right';
         if (missingPlaceRight > 0) {
            newDimensions = { width: computedSize.width - missingPlaceRight, height: computedSize.height };
         }
      }

      // arrange
      if (newDimensions) {
         computedSize = expandToBox(contentRatio, newDimensions);
         if (maxContentWidth) computedSize = maxWidth(computedSize, maxContentWidth);
      }
   }

   return (
      <div className={className}>
         <div className={classes.container}>
            {render(computedSize)}
            {showParticipants && (
               <PresentationSceneParticipants
                  location={participantsPlace!}
                  tileHeight={participantTileHeight}
                  tileWidth={participantTileWidth}
               />
            )}
         </div>
      </div>
   );
}
