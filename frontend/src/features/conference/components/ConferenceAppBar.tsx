import {
   AppBar,
   Box,
   Button,
   ButtonBase,
   createStyles,
   IconButton,
   makeStyles,
   Toolbar,
   Tooltip,
   Typography,
} from '@material-ui/core';
import MenuIcon from '@material-ui/icons/Menu';
import React from 'react';
import { useDispatch } from 'react-redux';
import { signOut } from 'src/features/auth/authSlice';
import usePermission, { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/hooks/usePermission';
import to from 'src/utils/to';
import * as coreHub from 'src/core-hub';
import { openSettings } from 'src/features/settings/settingsSlice';
import SettingsIcon from '@material-ui/icons/Settings';

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
   }),
);

export default function ConferenceAppBar() {
   const classes = useStyles();
   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => dispatch(openSettings());

   return (
      <AppBar position="static">
         <Toolbar>
            <IconButton edge="start" className={classes.menuButton} color="inherit" aria-label="menu">
               <MenuIcon />
            </IconButton>
            <Box flex={1}>
               <ButtonBase className={classes.title} {...to('/')}>
                  <Typography variant="h6">PaderConference</Typography>
               </ButtonBase>
            </Box>
            {canCloseConference && <Button onClick={handleCloseConference}>Close Conference</Button>}
            <Button onClick={handleSignOut}>Sign out</Button>
            <Tooltip title="Open Settings" aria-label="open settings">
               <IconButton aria-label="settings" color="inherit" onClick={handleOpenSettings}>
                  <SettingsIcon />
               </IconButton>
            </Tooltip>
         </Toolbar>
      </AppBar>
   );
}
