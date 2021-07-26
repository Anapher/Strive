import { Typography } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import { useSelector } from 'react-redux';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { selectBreakoutRoomState } from 'src/features/breakout-rooms/selectors';
import { BreakoutRoomScene, RenderSceneProps } from '../../types';
import AutoSceneLayout from '../AutoSceneLayout';

export default function RenderBreakoutRoom({ className, dimensions }: RenderSceneProps<BreakoutRoomScene>) {
   const state = useSelector(selectBreakoutRoomState);
   if (!state) return null;

   return (
      <AutoSceneLayout className={className} {...dimensions} center>
         <div>
            {state.deadline && (
               <Typography variant="h1">
                  <Countdown date={DateTime.fromISO(state.deadline).toJSDate()} renderer={CountdownRenderer} />
               </Typography>
            )}
            <Typography variant="h5">{state.description}</Typography>
         </div>
      </AutoSceneLayout>
   );
}
