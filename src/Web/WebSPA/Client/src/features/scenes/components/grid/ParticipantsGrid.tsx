import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { selectParticipants } from 'src/features/conference/selectors';
import { Participant } from 'src/features/conference/types';
import { selectParticipantsOfRoomWebcamAvailable } from 'src/features/media/selectors';
import { selectParticipantsOfCurrentRoom } from 'src/features/rooms/selectors';
import { Size } from 'src/types';
import { selectSceneOptions } from '../../selectors';
import { GridScene, RenderSceneProps } from '../../types';
import ActiveChipsLayout from '../ActiveChipsLayout';
import RenderGrid from './RenderGrid';

const GRID_MARGIN = 16;

const useStyles = makeStyles(() => ({
   grid: {
      padding: GRID_MARGIN,
   },
}));

export default function ParticipantsGrid({ dimensions, className }: RenderSceneProps<GridScene>) {
   const classes = useStyles();
   const participants = useSelector(selectParticipants);

   let visibleParticipants = useSelector(selectParticipantsOfCurrentRoom)
      .map((id) => participants[id])
      .filter((x): x is Participant => Boolean(x));
   const options = useSelector(selectSceneOptions);
   const participantsWithWebcam = useSelector(selectParticipantsOfRoomWebcamAvailable);

   const dimensionsWithMargin: Size = {
      width: dimensions.width - GRID_MARGIN * 2,
      height: dimensions.height - GRID_MARGIN * 2,
   };

   if (options?.hideParticipantsWithoutWebcam) {
      visibleParticipants = visibleParticipants.filter((x) => participantsWithWebcam.includes(x.id));

      return (
         <ActiveChipsLayout className={className}>
            <RenderGrid
               width={dimensionsWithMargin.width}
               height={dimensionsWithMargin.height - 32}
               participants={visibleParticipants}
               className={classes.grid}
            />
         </ActiveChipsLayout>
      );
   }

   return <RenderGrid {...dimensionsWithMargin} participants={visibleParticipants} className={classes.grid} />;
}
