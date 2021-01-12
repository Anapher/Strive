import {
   Avatar,
   IconButton,
   List,
   ListItem,
   ListItemAvatar,
   ListItemSecondaryAction,
   ListItemText,
} from '@material-ui/core';
import DeleteIcon from '@material-ui/icons/Delete';
import PersonIcon from '@material-ui/icons/Person';
import { Skeleton } from '@material-ui/lab';
import React, { useEffect } from 'react';
import { UseFormMethods } from 'react-hook-form';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ConferenceDataForm } from '../form';
import { loadUserInfo } from '../reducer';

type Props = {
   form: UseFormMethods<ConferenceDataForm>;
};

export default function TabModerators({ form: { watch, setValue } }: Props) {
   const moderators: string[] = watch('configuration.moderators');
   const userInfos = useSelector((state: RootState) => state.createConference.userInfo);
   const dispatch = useDispatch();

   useEffect(() => {
      dispatch(loadUserInfo(moderators));
   }, [JSON.stringify(moderators)]);

   const handeDeleteUser = (id: string) => {
      setValue(
         'configuration.moderators',
         moderators.filter((x) => x !== id),
      );
   };

   return (
      <List>
         {moderators.map((id) => {
            const info = userInfos.find((x) => x.id === id);
            if (!info)
               return (
                  <ListItem key={id}>
                     <ListItemAvatar>
                        <Skeleton variant="circle">
                           <Avatar />
                        </Skeleton>
                     </ListItemAvatar>
                     <ListItemText primary={<Skeleton variant="text" />} />
                  </ListItem>
               );

            return (
               <ListItem key={id}>
                  <ListItemAvatar>
                     <Avatar>
                        <PersonIcon />
                     </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                     primaryTypographyProps={{ color: info.notFound ? 'error' : undefined }}
                     primary={info.notFound ? `User with id "${info.id}" not found` : info.displayName}
                  />
                  <ListItemSecondaryAction>
                     <IconButton edge="end" aria-label="delete" onClick={() => handeDeleteUser(id)}>
                        <DeleteIcon />
                     </IconButton>
                  </ListItemSecondaryAction>
               </ListItem>
            );
         })}
      </List>
   );
}
