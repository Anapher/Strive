import { Box, Button, ButtonGroup, Typography } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown, { CountdownRenderProps } from 'react-countdown';
import { useDispatch, useSelector } from 'react-redux';
import { changeBreakoutRooms, closeBreakoutRooms } from 'src/core-hub';
import usePermission, { ROOMS_CAN_CREATE_REMOVE } from 'src/hooks/usePermission';
import { setCreationDialogOpen } from '../reducer';
import { selectBreakoutRoomState } from '../selectors';

const renderer = ({ hours, formatted }: CountdownRenderProps) => {
   let result = '';
   if (hours > 0) result = `${formatted.hours}:${formatted.minutes}:${formatted.seconds}`;
   else result = `${formatted.minutes}:${formatted.seconds}`;

   return <span>{result}</span>;
};

export default function ActiveBreakoutRoomsPopper() {
   const state = useSelector(selectBreakoutRoomState);
   const dispatch = useDispatch();

   if (!state) return <div />;

   const deadline = state.deadline ? DateTime.fromISO(state.deadline) : undefined;

   const handleAddMinutes = (minutes: number) => () => {
      if (!deadline) return;
      const newDuration = deadline.diffNow().plus({ minutes }).toISO();
      dispatch(changeBreakoutRooms([{ path: '/duration', op: 'replace', value: newDuration }]));
   };

   const handleCloseBreakoutRooms = () => {
      dispatch(closeBreakoutRooms());
   };

   const handleUpdateBreakoutRooms = () => {
      dispatch(setCreationDialogOpen(true));
   };

   const canModify = usePermission(ROOMS_CAN_CREATE_REMOVE);

   return (
      <div>
         <Box display="flex" justifyContent="space-between" alignItems="center">
            <Typography variant="h6">{state.amount} breakout rooms are open</Typography>
            {canModify && (
               <Button color="secondary" variant="contained" size="small" onClick={handleCloseBreakoutRooms}>
                  Close
               </Button>
            )}
         </Box>
         {deadline && (
            <Box mt={2}>
               <Typography gutterBottom>
                  Deadline at {deadline.toLocaleString(DateTime.TIME_24_SIMPLE)} (
                  <Countdown date={deadline.toJSDate()} renderer={renderer} />)
               </Typography>
               {canModify && (
                  <ButtonGroup variant="contained" aria-label="add time button group" size="small">
                     <Button onClick={handleAddMinutes(1)}>+1 min</Button>
                     <Button onClick={handleAddMinutes(5)}>+5 min</Button>
                     <Button onClick={handleAddMinutes(10)}>+10 min</Button>
                  </ButtonGroup>
               )}
            </Box>
         )}
         {canModify && (
            <Box mt={2}>
               <Button variant="contained" color="primary" onClick={handleUpdateBreakoutRooms}>
                  Change breakout rooms...
               </Button>
            </Box>
         )}
      </div>
   );
}
