import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { Size } from 'src/types';
import { selectHideParticipantsWithoutWebcam, selectParticipantGridList } from '../../selectors';
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
   const visibleParticipants = useSelector(selectParticipantGridList);
   const hideParticipantsWithoutWebcam = useSelector(selectHideParticipantsWithoutWebcam);

   const dimensionsWithMargin: Size = {
      width: dimensions.width - GRID_MARGIN * 2,
      height: dimensions.height - GRID_MARGIN * 2,
   };

   if (hideParticipantsWithoutWebcam) {
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
