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
import SettingsIcon from '@material-ui/icons/Settings';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { signOut } from 'src/features/auth/authSlice';
import { openSettings } from 'src/features/settings/settingsSlice';
import usePermission, { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/hooks/usePermission';
import { RootState } from 'src/store';
import to from 'src/utils/to';
import { setParticipantsOpen } from '../conferenceSlice';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';

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
   }),
);

type Props = {
   hamburgerRef: React.Ref<HTMLButtonElement>;
};

export default function ConferenceAppBar({ hamburgerRef }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => dispatch(openSettings());
   const participantsOpen = useSelector((state: RootState) => state.conference.participantsOpen);

   const handleToggleParticipantsOpen = () => dispatch(setParticipantsOpen(!participantsOpen));

   return (
      <AppBar position="static">
         <Toolbar variant="dense">
            <IconButton
               ref={hamburgerRef}
               edge="start"
               className={classes.menuButton}
               color="inherit"
               aria-label="participants menu"
               onClick={handleToggleParticipantsOpen}
            >
               {participantsOpen ? (
                  <ArrowBackIcon className={classes.noPointerEvents} />
               ) : (
                  <MenuIcon className={classes.noPointerEvents} />
               )}
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
