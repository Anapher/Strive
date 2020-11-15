import {
   Button,
   Divider,
   IconButton,
   List,
   ListItem,
   ListItemIcon,
   ListItemSecondaryAction,
   ListItemText,
   makeStyles,
   Radio,
   Typography,
} from '@material-ui/core';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import DesktopWindowsIcon from '@material-ui/icons/DesktopWindows';
import FileIcon from '@material-ui/icons/InsertDriveFile';
import StarIcon from '@material-ui/icons/Star';
import { Pin, PinOff } from 'mdi-material-ui';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import * as coreHub from 'src/core-hub';
import { selectAccessToken } from 'src/features/auth/selectors';
import ParticipantItem from 'src/features/conference/components/ParticipantItem';
import SceneManagement from 'src/features/scenes/components/SceneManagement';
import usePermission, { ROOMS_CAN_CREATE_REMOVE } from 'src/hooks/usePermission';
import { RootState } from 'src/store';
import { selectRoomViewModels } from '../selectors';
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
   const accessInfo = useSelector(selectAccessToken);

   const canCreateRemove = usePermission(ROOMS_CAN_CREATE_REMOVE);

   const dispatch = useDispatch();
   const handleSwitchRoom = (roomId: string) => dispatch(coreHub.switchRoom({ roomId }));

   return (
      <div className={classes.root}>
         <div className={classes.header}>
            <Typography variant="subtitle2">Participants</Typography>
            <IconButton size="small" onClick={onTogglePinned}>
               {pinned ? <Pin fontSize="small" /> : <PinOff fontSize="small" />}
            </IconButton>
         </div>
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
         <Divider style={{ marginLeft: 16 }} />
         <SceneManagement />
         {/* {canCreateRemove && <RoomManagement className={classes.roomManagement} />} */}
      </div>
   );
}
