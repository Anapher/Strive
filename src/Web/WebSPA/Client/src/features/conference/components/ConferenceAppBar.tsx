import { useReactOidc } from '@axa-fr/react-oidc-context';
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
   Typography,
} from '@material-ui/core';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import SettingsIcon from '@material-ui/icons/Settings';
import clsx from 'classnames';
import React, { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { useParams } from 'react-router';
import * as coreHub from 'src/core-hub';
import { openDialogToPatchAsync } from 'src/features/create-conference/reducer';
import { openSettings } from 'src/features/settings/reducer';
import usePermission from 'src/hooks/usePermission';
import { CONFERENCE_CAN_OPEN_AND_CLOSE } from 'src/permissions';
import { ConferenceRouteParams } from 'src/routes/types';
import { RootState } from 'src/store';
import { selectParticipantList } from '../selectors';
import AppBarLogo from './appbar/AppBarLogo';
import BreakoutRoomChips from './appbar/BreakoutRoomChip';
import WebRtcStatusChip from './appbar/WebRtcStatusChip';

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
   }),
);

type Props = {
   chatWidth: number;
};

export default function ConferenceAppBar({ chatWidth }: Props) {
   const classes = useStyles();
   const dispatch = useDispatch();
   const { t } = useTranslation();

   const { id: conferenceId } = useParams<ConferenceRouteParams>();

   const canCloseConference = usePermission(CONFERENCE_CAN_OPEN_AND_CLOSE);
   const handleCloseConference = () => dispatch(coreHub.closeConference());
   const handleOpenSettings = () => dispatch(openSettings());

   const { logout, oidcUser } = useReactOidc();
   const [isMenuOpen, setIsMenuOpen] = useState(false);

   const handleOpenMenu = () => setIsMenuOpen(true);
   const handleCloseMenu = () => setIsMenuOpen(false);

   const moreIconButtonRef = useRef<HTMLButtonElement>(null);
   const participants = useSelector(selectParticipantList);

   const breakoutRoomState = useSelector((state: RootState) => state.breakoutRooms.synchronized?.active);

   const handleShowPermissions = () => {
      dispatch(coreHub.fetchPermissions(null));
      handleCloseMenu();
   };

   const handlePatchConference = () => {
      if (!conferenceId) {
         console.error('Conference id must not be null');
         return;
      }
      dispatch(openDialogToPatchAsync(conferenceId));
      handleCloseMenu();
   };

   return (
      <AppBar position="static">
         <Toolbar variant="dense" className={classes.toolbar}>
            <Box flex={1}>
               <AppBarLogo />
            </Box>
            <Box>
               {breakoutRoomState && (
                  <BreakoutRoomChips
                     className={clsx(classes.chip, classes.breakoutRoomChip)}
                     state={breakoutRoomState}
                  />
               )}
               <WebRtcStatusChip />
               {participants && (
                  <Chip
                     className={classes.chip}
                     label={t('conference.appbar.participants', { count: participants.length })}
                     size="small"
                  />
               )}
            </Box>
            <Box
               width={chatWidth - 24 /** padding toolbar */}
               display="flex"
               alignItems="center"
               justifyContent="flex-end"
            >
               {oidcUser && (
                  <Box mr={2}>
                     <Typography variant="caption">
                        {t('conference.appbar.signed_in_as')} <b>{oidcUser.profile.name}</b>
                     </Typography>
                  </Box>
               )}
               <IconButton aria-label={t('common:settings')} color="inherit" onClick={handleOpenSettings}>
                  <SettingsIcon />
               </IconButton>
               <IconButton aria-label="more" color="inherit" onClick={handleOpenMenu} ref={moreIconButtonRef}>
                  <MoreVertIcon />
               </IconButton>
            </Box>

            <Menu open={isMenuOpen} onClose={handleCloseMenu} anchorEl={moreIconButtonRef.current}>
               <MenuItem onClick={handleShowPermissions}>{t('conference.appbar.show_my_permissions')}</MenuItem>

               <MenuItem onClick={handlePatchConference}>{t('conference.appbar.change_conference_settings')}</MenuItem>
               {canCloseConference && (
                  <MenuItem onClick={handleCloseConference}>{t('conference.appbar.close_conference')}</MenuItem>
               )}
               <MenuItem onClick={logout as any}>{t('common:sign_out')}</MenuItem>
            </Menu>
         </Toolbar>
      </AppBar>
   );
}
