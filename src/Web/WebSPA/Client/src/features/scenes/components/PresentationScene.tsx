import { makeStyles } from '@material-ui/core';
import React, { useEffect } from 'react';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import { expandToBox, maxWidth } from '../calculations';
import ActiveChipsLayout from './ActiveChipsLayout';
import PresentationSceneParticipants from './PresentationSceneParticipants';
import clsx from 'classnames';

const useStyles = makeStyles((theme) => ({
   container: {
      width: '100%',
      height: '100%',
      position: 'relative',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'flex-start',
      flexDirection: 'column',
      overflow: 'hidden',
   },
   chips: {
      marginTop: theme.spacing(1),
      marginBottom: theme.spacing(1),
      paddingRight: theme.spacing(2),
   },
}));

export type PresentationSceneProps = {
   className?: string;
   dimensions: Size;

   contentRatio: Size;
   maxContentWidth?: number;

   showParticipants?: boolean;
   fixedParticipants?: Participant[];

   participantTileWidth?: number;
   participantTileHeight?: number;

   maxOverlayFactor?: number;

   render: (size: Size, style: React.CSSProperties) => React.ReactNode;

   canShowParticipantsWithoutResize?: (canShow: boolean) => void;
};

export default function PresentationScene({
   className,
   contentRatio,
   maxContentWidth,
   dimensions,
   render,
   showParticipants = true,
   fixedParticipants,
   participantTileWidth = 16 * 18,
   participantTileHeight = 9 * 18,
   maxOverlayFactor = 0.3,
   canShowParticipantsWithoutResize,
}: PresentationSceneProps) {
   const classes = useStyles();

   dimensions = { ...dimensions, height: dimensions.height - 40 };

   // measure
   let computedSize = expandToBox(contentRatio, dimensions);
   if (maxContentWidth) computedSize = maxWidth(computedSize, maxContentWidth);

   let participantsPlace: 'bottom' | 'right' | undefined;

   console.log(dimensions.width);
   console.log(computedSize.width);

   // compute the vertical/horizontal margin if we would use the full size content
   const marginBottom = dimensions.height - computedSize.height;
   const marginRight = dimensions.width - computedSize.width;

   const neededPlaceBottom = (participantTileHeight + 20) * (1 - maxOverlayFactor);
   const neededPlaceRight = (participantTileWidth + 12) * (1 - maxOverlayFactor);

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

   useEffect(() => {
      if (canShowParticipantsWithoutResize) canShowParticipantsWithoutResize(newDimensions === undefined);
   }, [newDimensions === undefined]);

   // arrange
   if (newDimensions && showParticipants) {
      computedSize = expandToBox(contentRatio, newDimensions);
      if (maxContentWidth) computedSize = maxWidth(computedSize, maxContentWidth);
   }

   return (
      <ActiveChipsLayout className={clsx(className, classes.container)}>
         {render(computedSize, { marginRight: participantsPlace === 'right' ? neededPlaceRight : undefined })}
         {showParticipants && (
            <PresentationSceneParticipants
               location={participantsPlace}
               tileHeight={participantTileHeight}
               tileWidth={participantTileWidth}
               fixedParticipants={fixedParticipants}
            />
         )}
      </ActiveChipsLayout>
   );
}
