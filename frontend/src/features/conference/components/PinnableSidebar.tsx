import { ClickAwayListener, Drawer, fade, makeStyles, Paper } from '@material-ui/core';
import { useAnimation } from 'framer-motion';
import React, { useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import RoomsList from 'src/features/rooms/components/RoomsList';
import { RootState } from 'src/store';
import { setParticipantsOpen } from '../conferenceSlice';
import clsx from 'classnames';

const drawerWidth = 200;

const useStyles = makeStyles((theme) => ({
   drawer: {
      width: drawerWidth,
      flexShrink: 0,
      whiteSpace: 'nowrap',

      backgroundColor: fade(theme.palette.background.paper, 0.5),
      borderTopLeftRadius: 0,
      borderBottomLeftRadius: 0,
   },
   drawerOpen: {
      width: drawerWidth,
      transition: theme.transitions.create('width', {
         easing: theme.transitions.easing.sharp,
         duration: theme.transitions.duration.enteringScreen,
      }),
   },
   drawerClose: {
      transition: theme.transitions.create('width', {
         easing: theme.transitions.easing.sharp,
         duration: theme.transitions.duration.leavingScreen,
      }),
      overflowX: 'hidden',
      width: 0,
      [theme.breakpoints.up('sm')]: {
         width: 0,
      },
   },
   verticalButton: {
      marginTop: theme.spacing(2),
      top: 16,
      left: 0,
      position: 'absolute',
      transform: 'translate(-32px)',
      zIndex: 1000,
   },
}));

type Props = {
   pinned: boolean;
   onTogglePinned: () => void;
   getHamburger: () => HTMLButtonElement | null;
};

export default function PinnableSidebar({ pinned, onTogglePinned, getHamburger }: Props) {
   const classes = useStyles();

   const dispatch = useDispatch();
   const open = useSelector((state: RootState) => state.conference.participantsOpen);
   const setOpen = (visible: boolean) => dispatch(setParticipantsOpen(visible));

   const handleClickAway = (event: React.MouseEvent<Document, MouseEvent>) => {
      if (pinned) return;

      const button = getHamburger();
      if (button?.contains(event.target as HTMLElement)) {
         return;
      }

      if (!pinned) {
         setOpen(false);
      }
   };

   return (
      <ClickAwayListener onClickAway={handleClickAway}>
         <Paper
            className={clsx(classes.drawer, {
               [classes.drawerOpen]: open,
               [classes.drawerClose]: !open,
            })}
            elevation={1}
         >
            <RoomsList pinned={pinned} onTogglePinned={onTogglePinned} />
         </Paper>
      </ClickAwayListener>
   );
}
