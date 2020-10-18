import { makeStyles } from '@material-ui/core';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectAccessToken } from 'src/features/auth/selectors';
import ParticipantItem from 'src/features/conference/components/ParticipantItem';
import usePermission, { ROOMS_CAN_CREATE_REMOVE } from 'src/hooks/usePermission';
import { RootState } from 'src/store';
import { send } from 'src/store/conference-signal/actions';
import { selectRooms } from '../selectors';
import RoomHeader from './RoomHeader';
import RoomManagement from './RoomManagement';

const useStyles = makeStyles((theme) => ({
   root: {
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
   },
   room: {
      margin: theme.spacing(1, 1),
      flex: 1,
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
}));

export default function RoomsList() {
   const classes = useStyles();

   const rooms = useSelector(selectRooms);
   const participants = useSelector((state: RootState) => state.conference.participants);
   const accessInfo = useSelector(selectAccessToken);

   const canCreateRemove = usePermission(ROOMS_CAN_CREATE_REMOVE);

   const dispatch = useDispatch();
   const handleSwitchRoom = (roomId: string) => dispatch(send('SwitchRoom', { roomId }));

   return (
      <div className={classes.root}>
         <div className={classes.rooms}>
            {rooms?.map((x) => (
               <div key={x.roomId} className={classes.room}>
                  <RoomHeader
                     className={classes.roomHeader}
                     room={x}
                     selected={accessInfo && x.participants.includes(accessInfo.nameid)}
                     onClick={() => handleSwitchRoom(x.roomId)}
                  />
                  {x.participants.map((id) => (
                     <ParticipantItem key={id} participant={participants?.find((x) => id === x.participantId)} />
                  ))}
               </div>
            ))}
         </div>
         {canCreateRemove && <RoomManagement className={classes.roomManagement} />}
      </div>
   );
}
