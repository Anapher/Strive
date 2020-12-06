import {
   Box,
   Button,
   ButtonGroup,
   ClickAwayListener,
   Grow,
   ListItem,
   ListItemIcon,
   ListItemText,
   MenuItem,
   Paper,
   Popper,
   Typography,
} from '@material-ui/core';
import GroupWorkIcon from '@material-ui/icons/GroupWork';
import { DateTime } from 'luxon';
import React, { useRef, useState } from 'react';
import Countdown, { CountdownRenderProps } from 'react-countdown';
import { useDispatch, useSelector } from 'react-redux';
import { changeBreakoutRooms, closeBreakoutRooms } from 'src/core-hub';
import { setCreationDialogOpen } from '../breakoutRoomsSlice';
import { selectBreakoutRoomState, selectIsBreakoutRoomsOpen } from '../selectors';

type Props = {
   onClose: () => void;
};

export function BreakoutRoomAction({ onClose }: Props) {
   const dispatch = useDispatch();

   const handleOpen = () => {
      dispatch(setCreationDialogOpen(true));
      onClose();
   };

   const isOpen = useSelector(selectIsBreakoutRoomsOpen);

   return (
      <MenuItem onClick={handleOpen} disabled={isOpen}>
         <GroupWorkIcon fontSize="small" style={{ marginRight: 16 }} />
         Breakout Rooms
      </MenuItem>
   );
}

export function BreakoutRoomActive() {
   const isOpen = useSelector(selectIsBreakoutRoomsOpen);

   return <>{isOpen && <BreakoutRoomActiveItem />}</>;
}

function BreakoutRoomActiveItem() {
   const [optionsOpen, setOptionsOpen] = useState(false);
   const listItemRef = useRef(null);

   const handleOpen = () => setOptionsOpen(true);
   const handleClose = () => setOptionsOpen(false);

   return (
      <>
         <ListItem
            button
            style={{ paddingLeft: 16, paddingRight: 8 }}
            ref={listItemRef}
            onClick={handleOpen}
            selected={optionsOpen}
         >
            <ListItemIcon style={{ minWidth: 32 }}>
               <GroupWorkIcon />
            </ListItemIcon>
            <ListItemText primary="Breakout Rooms" />
         </ListItem>
         <Popper open={optionsOpen} anchorEl={listItemRef.current} transition placement="right-end">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps} style={{ transformOrigin: 'left bottom' }}>
                  <Paper style={{ width: 400 }}>
                     <ClickAwayListener onClickAway={handleClose}>
                        <Box p={2}>
                           <BreakoutRoomPopper />
                        </Box>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </>
   );
}

const renderer = ({ hours, formatted }: CountdownRenderProps) => {
   let result = '';
   if (hours > 0) result = `${formatted.hours}:${formatted.minutes}:${formatted.seconds}`;
   else result = `${formatted.minutes}:${formatted.seconds}`;

   return <span>{result}</span>;
};

function BreakoutRoomPopper() {
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

   return (
      <div>
         <Box display="flex" justifyContent="space-between" alignItems="center">
            <Typography variant="h6">{state.amount} breakout rooms are open</Typography>
            <Button color="secondary" variant="contained" size="small" onClick={handleCloseBreakoutRooms}>
               Close
            </Button>
         </Box>
         {deadline && (
            <Box mt={2}>
               <Typography gutterBottom>
                  Deadline at {deadline.toLocaleString(DateTime.TIME_24_SIMPLE)} (
                  <Countdown date={deadline.toJSDate()} renderer={renderer} />)
               </Typography>
               <ButtonGroup variant="contained" aria-label="add time button group" size="small">
                  <Button onClick={handleAddMinutes(1)}>+1 min</Button>
                  <Button onClick={handleAddMinutes(5)}>+5 min</Button>
                  <Button onClick={handleAddMinutes(10)}>+10 min</Button>
               </ButtonGroup>
            </Box>
         )}
         <Box mt={2}>
            <Button variant="contained" color="primary" onClick={handleUpdateBreakoutRooms}>
               Change breakout rooms...
            </Button>
         </Box>
      </div>
   );
}
