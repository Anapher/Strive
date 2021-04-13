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
import { Controller, ControllerRenderProps, UseFormReturn } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from 'src/store';
import { ConferenceDataForm } from '../form';
import { loadUserInfo } from '../reducer';

type Props = {
   form: UseFormReturn<ConferenceDataForm>;
};

export default function TabModerators({ form: { control } }: Props) {
   return (
      <Controller
         control={control}
         name="configuration.moderators"
         render={({ field }) => <ModeratorList field={field} />}
      />
   );
}

type ModeratorListProps = {
   field: ControllerRenderProps<ConferenceDataForm, 'configuration.moderators'>;
};

function ModeratorList({ field: { value, onChange } }: ModeratorListProps) {
   const userInfos = useSelector((state: RootState) => state.createConference.userInfo);
   const dispatch = useDispatch();
   const { t } = useTranslation();

   useEffect(() => {
      dispatch(loadUserInfo(value));
   }, [JSON.stringify(value)]);

   const handeDeleteUser = (id: string) => {
      onChange(value.filter((x) => x !== id));
   };

   return (
      <List>
         {value.map((id) => {
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
                     primary={
                        info.notFound
                           ? t('dialog_create_conference.tabs.moderators.error_user_not_found', { id: info.id })
                           : info.displayName
                     }
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
