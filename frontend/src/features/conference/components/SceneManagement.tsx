import React, { useRef, useState } from 'react';
import {
   Button,
   ClickAwayListener,
   Grow,
   IconButton,
   List,
   ListItem,
   ListItemIcon,
   ListItemSecondaryAction,
   ListItemText,
   makeStyles,
   MenuItem,
   MenuList,
   Paper,
   Popper,
   Radio,
   Typography,
} from '@material-ui/core';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import FileIcon from '@material-ui/icons/InsertDriveFile';
import StarIcon from '@material-ui/icons/Star';
import GroupWorkIcon from '@material-ui/icons/GroupWork';

const useStyles = makeStyles((theme) => ({
   root: {
      padding: theme.spacing(1, 2),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
   },
}));

export default function SceneManagement() {
   const classes = useStyles();
   const [actionPopper, setActionPopper] = useState(false);
   const actionButton = useRef<HTMLButtonElement>(null);
   const handleClose = () => setActionPopper(false);
   const handleOpen = () => setActionPopper(true);

   const options = ['Breakout Rooms', 'Upload PDF', 'Poll'];

   return (
      <div>
         <List dense disablePadding>
            <li style={{ paddingLeft: 16, marginTop: 16 }}>
               <Typography variant="subtitle2" color="textSecondary">
                  Scenes
               </Typography>
            </li>
            <ListItem button style={{ paddingLeft: 16, paddingRight: 8 }}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <StarIcon />
               </ListItemIcon>
               <ListItemText primary="Automatic" />
               <ListItemSecondaryAction>
                  <Radio edge="end" />
               </ListItemSecondaryAction>
            </ListItem>
            <ListItem button style={{ paddingLeft: 16, paddingRight: 8 }}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <DesktopWindowsIcon />
               </ListItemIcon>
               <ListItemText primary="Your Screen" />
               <ListItemSecondaryAction>
                  <Radio edge="end" />
               </ListItemSecondaryAction>
            </ListItem>
            <ListItem button style={{ paddingLeft: 16, paddingRight: 8 }}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <DesktopWindowsIcon />
               </ListItemIcon>
               <ListItemText primary="Joseph's Screen" />
               <ListItemSecondaryAction>
                  <Radio edge="end" />
               </ListItemSecondaryAction>
            </ListItem>
            <ListItem button style={{ paddingLeft: 16, paddingRight: 8 }}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <FileIcon />
               </ListItemIcon>
               <ListItemText primary="Slides.pdf" />
               <ListItemSecondaryAction>
                  <Radio edge="end" />
               </ListItemSecondaryAction>
            </ListItem>
            <li style={{ paddingLeft: 16, marginTop: 16 }}>
               <Typography variant="subtitle2" color="textSecondary">
                  Active
               </Typography>
            </li>
            <ListItem button style={{ paddingLeft: 16, paddingRight: 8 }}>
               <ListItemIcon style={{ minWidth: 32 }}>
                  <GroupWorkIcon />
               </ListItemIcon>
               <ListItemText primary="Breakout Rooms" />
            </ListItem>
         </List>
         <Paper elevation={4} className={classes.root}>
            <Button variant="contained" color="primary" size="small" fullWidth ref={actionButton} onClick={handleOpen}>
               Actions <ArrowDropDownIcon />
            </Button>
         </Paper>
         <Popper open={actionPopper} anchorEl={actionButton.current} role={undefined} transition disablePortal>
            {({ TransitionProps }) => (
               <Grow
                  {...TransitionProps}
                  style={{
                     transformOrigin: 'center bottom',
                  }}
               >
                  <Paper>
                     <ClickAwayListener onClickAway={handleClose}>
                        <MenuList id="action list">
                           <MenuItem>
                              <GroupWorkIcon fontSize="small" style={{ marginRight: 16 }} />
                              Breakout Rooms
                           </MenuItem>
                           {options.map((option, index) => (
                              <MenuItem key={option}>{option}</MenuItem>
                           ))}
                        </MenuList>
                     </ClickAwayListener>
                  </Paper>
               </Grow>
            )}
         </Popper>
      </div>
   );
}
