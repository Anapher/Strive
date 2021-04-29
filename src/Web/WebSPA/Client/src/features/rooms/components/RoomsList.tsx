import { makeStyles } from '@material-ui/core';
import clsx from 'classnames';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import ParticipantItem from 'src/features/conference/components/ParticipantItem';
import { selectParticipants } from 'src/features/conference/selectors';
import SceneManagement from 'src/features/scenes/components/SceneManagement';
import useMyParticipantId from 'src/hooks/useMyParticipantId';
import { selectRoomViewModels } from '../selectors';
import RoomHeader from './RoomHeader';

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
      marginTop: theme.spacing(1),
   },
   roomHeader: {
      marginBottom: theme.spacing(0.5),
   },
   roomManagement: {
      marginTop: theme.spacing(1),
   },
}));

export default function RoomsList() {
   const classes = useStyles();

   const rooms = useSelector(selectRoomViewModels);
   const participants = useSelector(selectParticipants);
   const myId = useMyParticipantId();

   const dispatch = useDispatch();
   const handleSwitchRoom = (roomId: string) => dispatch(coreHub.switchRoom({ roomId }));

   return (
      <div className={classes.root}>
         <div className={classes.rooms}>
            {rooms?.map((room) => (
               <div
                  key={room.roomId}
                  className={clsx(classes.room, room.participants.length > 0 && classes.roomWithParticipants)}
               >
                  <RoomHeader
                     className={classes.roomHeader}
                     room={room}
                     selected={room.participants.includes(myId)}
                     onClick={() => handleSwitchRoom(room.roomId)}
                  />
                  <div className={classes.participants}>
                     {room.participants.map((id) => (
                        <ParticipantItem key={id} participant={participants.find((x) => id === x.id)} />
                     ))}
                  </div>
               </div>
            ))}
         </div>
         <SceneManagement />
      </div>
   );
}
