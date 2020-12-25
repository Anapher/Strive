import { Chip, Divider, Grid } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown, { CountdownRenderProps } from 'react-countdown';
import { ActiveBreakoutRoomState } from 'src/features/breakout-rooms/types';

const renderer = ({ hours, formatted }: CountdownRenderProps) => {
   let result = '';
   if (hours > 0) result = `${formatted.hours}:${formatted.minutes}:${formatted.seconds}`;
   else result = `${formatted.minutes}:${formatted.seconds}`;

   return <span>{result}</span>;
};

type Props = {
   state: ActiveBreakoutRoomState;
   className?: string;
};

export default function BreakoutRoomChip({ state, className }: Props) {
   const deadline = state.deadline ? DateTime.fromISO(state.deadline) : undefined;

   return (
      <Chip
         className={className}
         label={
            <Grid container>
               {state.description && (
                  <>
                     <span>{state.description}</span>
                     <Divider orientation="vertical" flexItem style={{ marginLeft: 8, marginRight: 8 }} />
                  </>
               )}
               <span>
                  Breakout Room phase
                  {deadline && (
                     <span>
                        {' '}
                        until {deadline.toLocaleString(DateTime.TIME_24_SIMPLE)} (
                        <Countdown date={deadline.toJSDate()} renderer={renderer} />)
                     </span>
                  )}
               </span>
            </Grid>
         }
         color="primary"
         size="small"
      />
   );
}
