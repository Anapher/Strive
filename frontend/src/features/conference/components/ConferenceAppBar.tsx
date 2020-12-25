import {
   AppBar,
   Box,
   Chip,
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
import { signOut } from 'src/features/auth/reducer';
import { selectAccessToken } from 'src/features/auth/selectors';
import { setOpen } from 'src/features/diagnostics/reducer';
import { openSettings } from 'src/features/settings/reducer';
import usePermission, { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/hooks/usePermission';
import { RootState } from 'src/store';
import useWebRtcStatus from 'src/store/webrtc/hooks/useWebRtcStatus';
import { selectParticipants } from '../selectors';
import BreakoutRoomChips from './appbar/BreakoutRoomChip';
import clsx from 'classnames';

const useStyles = makeStyles((theme) =>
   createStyles({
      root: {
         flexGrow: 1,
      },
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
      chip: {
         backgroundColor: 'rgb(55, 55, 57)',
         padding: theme.spacing(0, 1),
      },
      breakoutRoomChip: {
         marginRight: theme.spacing(1),
         backgroundColor: theme.palette.primary.dark,
      },
      errorChip: {
         backgroundColor: theme.palette.error.main,
         color: theme.palette.error.contrastText,
      },
   }),
);

type Props = {
   chatWidth: number;
};

export default function ConferenceAppBar({ chatWidth }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const handleSignOut = () => dispatch(signOut());

   const diagnosticsOpen = useSelector((state: RootState) => state.diagnostics.open);

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => dispatch(openSettings());

   const token = useSelector(selectAccessToken);
   const [isMenuOpen, setIsMenuOpen] = useState(false);

   const handleOpenMenu = () => setIsMenuOpen(true);
   const handleCloseMenu = () => setIsMenuOpen(false);

   const moreIconButtonRef = useRef<HTMLButtonElement>(null);
   const participants = useSelector(selectParticipants);
   const webRtcStatus = useWebRtcStatus();

   const breakoutRoomState = useSelector((state: RootState) => state.breakoutRooms.synchronized?.active);

   const handleShowPermissions = () => {
      dispatch(coreHub.fetchPermissions(null));
      handleCloseMenu();
   };

   const handleShowDiagnostics = () => {
      dispatch(setOpen(true));
      handleCloseMenu();
   };

   return (
      <AppBar position="static">
         <Toolbar variant="dense" className={classes.toolbar}>
            <Box flex={1}>
               <Typography variant="h6">PaderConference</Typography>
            </Box>
            <Box>
               {breakoutRoomState && (
                  <BreakoutRoomChips
                     className={clsx(classes.chip, classes.breakoutRoomChip)}
                     state={breakoutRoomState}
                  />
               )}
               {webRtcStatus !== 'connected' && (
                  <Chip
                     className={classes.errorChip}
                     style={{ marginRight: 8 }}
                     label={
                        webRtcStatus === 'connecting' ? 'Reconnecting to WebRTC server...' : 'WebRTC not initialized'
                     }
                     size="small"
                  />
               )}
               {participants && (
                  <Chip className={classes.chip} label={`Participants: ${participants.length}`} size="small" />
               )}
            </Box>
            <Box
               width={chatWidth - 24 /** padding toolbar */}
               display="flex"
               alignItems="center"
               justifyContent="flex-end"
            >
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
            </Box>

            <Menu open={isMenuOpen} onClose={handleCloseMenu} anchorEl={moreIconButtonRef.current}>
               <MenuItem onClick={handleShowPermissions}>Show my permissions</MenuItem>
               <MenuItem onClick={handleShowDiagnostics} disabled={diagnosticsOpen}>
                  Diagnostics
               </MenuItem>
               {canCloseConference && <MenuItem onClick={handleCloseConference}>Close Conference</MenuItem>}
               <MenuItem onClick={handleSignOut}>Sign out</MenuItem>
            </Menu>
         </Toolbar>
      </AppBar>
   );
}
