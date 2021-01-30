import { Divider, IconButton, makeStyles, Tooltip, Typography } from '@material-ui/core';
import { Pin, PinOff } from 'mdi-material-ui';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import ParticipantItem from 'src/features/conference/components/ParticipantItem';
import SceneManagement from 'src/features/scenes/components/SceneManagement';
import { RootState } from 'src/store';
import { selectRoomViewModels } from '../selectors';
import RoomHeader from './RoomHeader';
import clsx from 'classnames';
import useMyParticipantId from 'src/hooks/useMyParticipantId';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
   room: {
      flex: 1,
      margin: theme.spacing(0, 1),
   },
   roomWithParticipants: {
      marginBottom: theme.spacing(1),
   },
   participants: {
      marginLeft: theme.spacing(1),
   },
   rooms: {
      flex: 1,
   },
   roomHeader: {
      marginBottom: theme.spacing(0.5),
   },
   roomManagement: {
      marginTop: theme.spacing(1),
   },
   header: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      margin: theme.spacing(1, 1, 0, 1),
   },
}));

type Props = {
   pinned: boolean;
   onTogglePinned: () => void;
};

export default function RoomsList({ pinned, onTogglePinned }: Props) {
   const classes = useStyles();

   const rooms = useSelector(selectRoomViewModels);
   const participants = useSelector((state: RootState) => state.conference.participants);
   const myId = useMyParticipantId();

   const dispatch = useDispatch();
   const handleSwitchRoom = (roomId: string) => dispatch(coreHub.switchRoom({ roomId }));

   return (
      <div className={classes.root}>
         <div className={classes.header}>
            <Typography variant="subtitle2">Participants</Typography>
            <Tooltip title={pinned ? 'Auto hide sidebar' : 'Pin sidebar'}>
               <IconButton size="small" onClick={onTogglePinned}>
                  {pinned ? <Pin fontSize="small" /> : <PinOff fontSize="small" />}
               </IconButton>
            </Tooltip>
         </div>
         <div className={classes.rooms}>
            {rooms?.map((x) => (
               <div
                  key={x.roomId}
                  className={clsx(classes.room, x.participants.length > 0 && classes.roomWithParticipants)}
               >
                  <RoomHeader
                     className={classes.roomHeader}
                     room={x}
                     selected={x.participants.includes(myId)}
                     onClick={() => handleSwitchRoom(x.roomId)}
                  />
                  <div className={classes.participants}>
                     {x.participants.map((id) => (
                        <ParticipantItem key={id} participant={participants?.find((x) => id === x.participantId)} />
                     ))}
                  </div>
               </div>
            ))}
         </div>
         <Divider style={{ marginLeft: 16 }} />
         <SceneManagement />
      </div>
   );
}
