import { Box, ClickAwayListener, Grow, ListItem, ListItemIcon, ListItemText, Paper, Popper } from '@material-ui/core';
import GroupWorkIcon from '@material-ui/icons/GroupWork';
import React, { useRef, useState } from 'react';
import ActiveBreakoutRoomsPopper from './ActiveBreakoutRoomsPopper';

export default function ActiveMenuItem() {
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
                           <ActiveBreakoutRoomsPopper />
                        </Box>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </>
   );
}
