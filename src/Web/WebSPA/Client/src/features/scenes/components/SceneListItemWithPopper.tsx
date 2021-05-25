import React, { useRef, useState } from 'react';
import SceneListItem, { Props as SceneListItemProps } from './SceneListItem';
import { Box, ClickAwayListener, Grow, IconButton, ListItemSecondaryAction, Paper, Popper } from '@material-ui/core';
import SettingsIcon from '@material-ui/icons/Settings';

type Props = SceneListItemProps & {
   children: React.ReactNode;
};

export default function SceneListItemWithPopper(props: Props) {
   const [open, setOpen] = useState(false);
   const listItemRef = useRef(null);

   const handleOpen = () => setOpen(true);
   const handleClose = () => setOpen(false);

   return (
      <>
         <SceneListItem {...props} ref={listItemRef}>
            <ListItemSecondaryAction>
               <IconButton edge="end" aria-label="options" onClick={handleOpen}>
                  <SettingsIcon />
               </IconButton>
            </ListItemSecondaryAction>
         </SceneListItem>
         <Popper open={open} anchorEl={listItemRef.current} transition placement="right-end">
            {({ TransitionProps }) => (
               <Grow {...TransitionProps} style={{ transformOrigin: 'left bottom' }}>
                  <Paper style={{ width: 400 }}>
                     <ClickAwayListener onClickAway={handleClose}>
                        <Box p={2}>{props.children}</Box>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </>
   );
}
