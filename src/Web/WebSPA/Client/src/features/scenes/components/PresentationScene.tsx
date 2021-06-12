import { makeStyles, Portal } from '@material-ui/core';
import clsx from 'classnames';
import React, { useContext } from 'react';
import ConferenceLayoutContext from 'src/features/conference/conference-layout-context';
import { Participant } from 'src/features/conference/types';
import { Size } from 'src/types';
import { expandToBox, maxWidth } from '../calculations';
import ActiveChipsLayout, { ACTIVE_CHIPS_LAYOUT_HEIGHT } from './ActiveChipsLayout';
import ActiveParticipantsGrid from './ActiveParticipantsGrid';
import PresentationSceneParticipants from './PresentationSceneParticipants';

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

/**
 * max size: the most important thing is that the content has a maximum size.
 * If there is no space for participant tiles in the scene, use the place over the chat
 *
 * default: place the participant tiles to the left or bottom of the scene and scale the content
 * if required
 */
type PresentationSceneVariant = 'max-size' | 'default';

export type PresentationSceneProps = {
   variant?: PresentationSceneVariant;

   className?: string;
   dimensions: Size;

   contentRatio: Size;
   maxContentWidth?: number;

   showParticipants?: boolean;
   fixedParticipants?: Participant[] | Participant;

   participantTileWidth?: number;
   participantTileHeight?: number;

   maxOverlayFactor?: number;

   render: (size: Size, style: React.CSSProperties) => React.ReactNode;
};

export default function PresentationScene({
   variant = 'default',
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
}: PresentationSceneProps) {
   const classes = useStyles();

   dimensions = { ...dimensions, height: dimensions.height - ACTIVE_CHIPS_LAYOUT_HEIGHT };

   // measure
   let computedSize = expandToBox(contentRatio, dimensions);
   if (maxContentWidth) computedSize = maxWidth(computedSize, maxContentWidth);

   let participantsPlace: 'bottom' | 'right' | undefined;

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

   if (fixedParticipants !== undefined && !Array.isArray(fixedParticipants)) {
      fixedParticipants = [fixedParticipants];
   }

   console.log('newDimensions', newDimensions);

   if (variant === 'max-size' && newDimensions) {
      return (
         <ActiveChipsLayout className={clsx(className, classes.container)}>
            {showParticipants && (
               <PortalWithParticipant
                  participant={fixedParticipants && fixedParticipants.length > 0 ? fixedParticipants[0] : undefined}
               />
            )}
            {render(computedSize, {})}
         </ActiveChipsLayout>
      );
   }

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

type PortalWithParticipantProps = {
   participant?: Participant;
};

function PortalWithParticipant({ participant }: PortalWithParticipantProps) {
   const context = useContext(ConferenceLayoutContext);

   return (
      <Portal container={context.chatContainer}>
         <ActiveParticipantsGrid width={context.chatWidth} participant={participant} />
      </Portal>
   );
}
