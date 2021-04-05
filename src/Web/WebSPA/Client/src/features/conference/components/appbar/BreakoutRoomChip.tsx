import { Chip, Divider, Grid } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { BreakoutRoomsConfig } from 'src/core-hub.types';

type Props = {
   state: BreakoutRoomsConfig;
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
                        <Countdown date={deadline.toJSDate()} renderer={CountdownRenderer} />)
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
