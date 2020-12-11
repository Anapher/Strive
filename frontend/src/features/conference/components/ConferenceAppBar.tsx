import {
   AppBar,
   Box,
   createStyles,
   IconButton,
   makeStyles,
   Menu,
   MenuItem,
   Toolbar,
   Tooltip,
   Typography,
} from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import SettingsIcon from '@material-ui/icons/Settings';
import React, { useRef, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { signOut } from 'src/features/auth/authSlice';
import { selectAccessToken } from 'src/features/auth/selectors';
import { openSettings } from 'src/features/settings/settingsSlice';
import usePermission, { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/hooks/usePermission';

const useStyles = makeStyles((theme) =>
   createStyles({
      root: {
         flexGrow: 1,
      },
      menuButton: {},
      title: {
         padding: theme.spacing(1, 2),
         borderRadius: theme.shape.borderRadius,
      },
      noPointerEvents: {
         pointerEvents: 'none',
      },
      toolbar: {
         backgroundColor: 'rgb(35, 35, 37)',
      },
   }),
);

export default function ConferenceAppBar() {
   const classes = useStyles();
   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => dispatch(openSettings());

   const token = useSelector(selectAccessToken);
   const [isMenuOpen, setIsMenuOpen] = useState(false);

   const handleOpenMenu = () => setIsMenuOpen(true);
   const handleCloseMenu = () => setIsMenuOpen(false);

   const moreIconButtonRef = useRef<HTMLButtonElement>(null);

   return (
      <AppBar position="static">
         <Toolbar variant="dense" className={classes.toolbar}>
            <Box flex={1}>
               <Typography variant="h6">PaderConference</Typography>
            </Box>
            {token && (
               <Box mr={2}>
                  <Typography variant="caption">
                     Signed in as <b>{token.unique_name}</b>
                  </Typography>
               </Box>
            )}
            <Tooltip title="Open Settings" aria-label="open settings">
               <IconButton aria-label="settings" color="inherit" onClick={handleOpenSettings}>
                  <SettingsIcon />
               </IconButton>
            </Tooltip>
            <IconButton aria-label="more" color="inherit" onClick={handleOpenMenu} ref={moreIconButtonRef}>
               <MoreVertIcon />
            </IconButton>

            <Menu open={isMenuOpen} onClose={handleCloseMenu} anchorEl={moreIconButtonRef.current}>
               {canCloseConference && <MenuItem onClick={handleCloseConference}>Close Conference</MenuItem>}
               <MenuItem onClick={handleSignOut}>Sign out</MenuItem>
            </Menu>
         </Toolbar>
      </AppBar>
   );
}
