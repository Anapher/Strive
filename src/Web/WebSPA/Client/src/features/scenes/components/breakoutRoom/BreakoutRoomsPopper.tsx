import { Box, Button, ButtonGroup, Typography } from '@material-ui/core';
import { DateTime } from 'luxon';
import React from 'react';
import Countdown from 'react-countdown';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import CountdownRenderer from 'src/components/CountdownRenderer';
import { changeBreakoutRooms, closeBreakoutRooms } from 'src/core-hub';
import usePermission from 'src/hooks/usePermission';
import { ROOMS_CAN_CREATE_REMOVE } from 'src/permissions';
import { setCreationDialogOpen } from '../../../breakout-rooms/reducer';
import { selectBreakoutRoomState } from '../../../breakout-rooms/selectors';

export default function BreakoutRoomsPopper() {
   const dispatch = useDispatch();
   const { t } = useTranslation();
   const state = useSelector(selectBreakoutRoomState);
   const canModify = usePermission(ROOMS_CAN_CREATE_REMOVE);

   if (!state) return null;

   const deadline = state.deadline ? DateTime.fromISO(state.deadline) : undefined;

   const handleAddMinutes = (minutes: number) => () => {
      if (!deadline) return;
      const newDeadline = deadline.plus({ minutes }).toISO();
      dispatch(changeBreakoutRooms([{ path: '/deadline', op: 'add', value: newDeadline }]));
   };

   const handleCloseBreakoutRooms = () => {
      dispatch(closeBreakoutRooms());
   };

   const handleUpdateBreakoutRooms = () => {
      dispatch(setCreationDialogOpen(true));
   };

   return (
      <div>
         <Box display="flex" justifyContent="space-between" alignItems="center">
            <Typography variant="h6">{t('conference.scenes.breakout_rooms.title', { count: state.amount })}</Typography>
            {canModify && (
               <Button color="secondary" variant="contained" size="small" onClick={handleCloseBreakoutRooms}>
                  {t('common:close')}
               </Button>
            )}
         </Box>
         {deadline && (
            <Box mt={2}>
               <Typography gutterBottom>
                  {t('conference.scenes.breakout_rooms.deadline_description', {
                     date: deadline.toLocaleString(DateTime.TIME_24_SIMPLE),
                  }) + ' '}
                  (
                  <Countdown date={deadline.toJSDate()} renderer={CountdownRenderer} />)
               </Typography>
               {canModify && (
                  <ButtonGroup
                     variant="contained"
                     aria-label={t('conference.scenes.breakout_rooms.add_time_button_group')}
                     size="small"
                  >
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
                  {t('conference.scenes.breakout_rooms.change_breakout_rooms')}
               </Button>
            </Box>
         )}
      </div>
   );
}
