import { Button, ButtonGroup, makeStyles, Paper } from '@material-ui/core';
import CloseIcon from '@material-ui/icons/Close';
import clsx from 'classnames';
import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { send } from 'src/store/conference-signal/actions';
import { selectRooms } from '../selectors';
import BreakoutRoomDialog from './BreakoutRoomDialog';
import * as coreHub from 'src/core-hub';

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(1),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

type Props = {
   className?: string;
};

export default function RoomManagement({ className }: Props) {
   const classes = useStyles();
   const [breakoutRoomDialogOpen, setBreakoutRoomDialogOpen] = useState(false);
   const rooms = useSelector(selectRooms);
   const dispatch = useDispatch();

   const handleCloseBreakoutRooms = () => {
      if (!rooms) return;
      const breakoutRooms = rooms.filter((x) => !x.isDefaultRoom).map((x) => x.roomId);
      dispatch(coreHub.removeRooms(breakoutRooms));
   };

   return (
      <>
         <Paper elevation={4} className={clsx(classes.root, className)}>
            <ButtonGroup variant="contained" color="primary" size="small">
               <Button onClick={() => setBreakoutRoomDialogOpen(true)}>Breakout Rooms</Button>
               {rooms && rooms.length > 1 && (
                  <Button onClick={handleCloseBreakoutRooms}>
                     <CloseIcon fontSize="small" />
                  </Button>
               )}
            </ButtonGroup>
         </Paper>
         <BreakoutRoomDialog open={breakoutRoomDialogOpen} onClose={() => setBreakoutRoomDialogOpen(false)} />
      </>
   );
}
