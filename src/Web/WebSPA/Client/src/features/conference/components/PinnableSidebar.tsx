import { ClickAwayListener, fade, IconButton, makeStyles, Paper, Tooltip } from '@material-ui/core';
import clsx from 'classnames';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import RoomsList from 'src/features/rooms/components/RoomsList';
import { RootState } from 'src/store';
import { setParticipantsOpen } from '../reducer';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import { motion } from 'framer-motion';
import { Pin, PinOff } from 'mdi-material-ui';

const drawerWidth = 200;

const useStyles = makeStyles((theme) => ({
   drawer: {
      width: drawerWidth,
      flexShrink: 0,
      whiteSpace: 'nowrap',

      backgroundColor: fade(theme.palette.background.paper, 0.5),
      borderTopLeftRadius: 0,
      borderBottomLeftRadius: 0,
      height: '100%',
   },
   drawerOpen: {
      width: drawerWidth,
      borderColor: theme.palette.divider,
      borderWidth: '0px 1px 0px 0px',
      borderStyle: 'solid',
   },
   drawerClose: {
      overflowX: 'hidden',
      width: 0,
   },
   listRoot: {
      position: 'relative',
      height: '100%',
   },
   backArrowButton: {
      position: 'absolute',
      top: 8,
      right: -34,
      zIndex: theme.zIndex.drawer,
      display: 'flex',
      flexDirection: 'column',
   },
}));

type Props = {
   pinned: boolean;
   onTogglePinned: () => void;
};

const arrowVariants = {
   open: { rotateY: 0, translateX: 0 },
   closed: { rotateY: 180, translateX: -3 },
};

export default function PinnableSidebar({ pinned, onTogglePinned }: Props) {
   const classes = useStyles();

   const dispatch = useDispatch();
   const open = useSelector((state: RootState) => state.conference.participantsOpen);
   const setOpen = (visible: boolean) => dispatch(setParticipantsOpen(visible));

   const handleClickAway = () => {
      if (pinned) return;

      if (!pinned) {
         setOpen(false);
      }
   };

   const handleToggle = () => setOpen(!open);

   return (
      <ClickAwayListener onClickAway={handleClickAway}>
         <div className={classes.listRoot}>
            <Paper
               className={clsx(classes.drawer, {
                  [classes.drawerOpen]: open,
                  [classes.drawerClose]: !open,
               })}
               elevation={1}
            >
               <RoomsList />
            </Paper>
            <div className={classes.backArrowButton}>
               <IconButton aria-label="toggle room list" size="small" onClick={handleToggle}>
                  <ArrowBackIcon
                     component={motion.svg}
                     variants={arrowVariants}
                     animate={open ? 'open' : 'closed'}
                     style={{ transformOrigin: 'center', ...({ originX: 0.5, originY: 0.5 } as any) }}
                     fontSize="small"
                  />
               </IconButton>
               <Tooltip
                  title={pinned ? 'Auto hide sidebar' : 'Pin sidebar'}
                  style={{ display: open ? undefined : 'none' }}
               >
                  <IconButton size="small" onClick={onTogglePinned}>
                     {pinned ? <Pin fontSize="small" /> : <PinOff fontSize="small" />}
                  </IconButton>
               </Tooltip>
            </div>
         </div>
      </ClickAwayListener>
   );
}
