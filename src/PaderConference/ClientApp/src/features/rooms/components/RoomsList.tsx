import { Typography } from '@material-ui/core';
import React from 'react';
import { useSelector } from 'react-redux';
import { roomsSelector } from '../selectors';

export default function RoomsList() {
   const rooms = useSelector(roomsSelector);

   return (
      <div>
         {rooms?.map((x) => (
            <div key={x.roomId}>
               <Typography>{x.displayName}</Typography>
            </div>
         ))}
      </div>
   );
}
